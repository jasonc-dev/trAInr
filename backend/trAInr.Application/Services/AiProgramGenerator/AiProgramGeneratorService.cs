using System.Text.Json;
using System.Text.Json.Serialization;
using trAInr.Application.DTOs.AI;
using trAInr.Application.DTOs.ProgramTemplate;
using trAInr.Application.Interfaces;
using trAInr.Application.Interfaces.Repositories;
using trAInr.Application.Interfaces.Services.AI;
using trAInr.Domain.Aggregates;
using trAInr.Domain.Entities;

namespace trAInr.Application.Services.AiProgramGenerator;

public class AiProgramGeneratorService(IOpenAiClient openAiClient,
IUnitOfWork unitOfWork,
IProgramTemplateRepository programTemplateRepository,
IExerciseRetrievalService exerciseRetrievalService) : IAiProgramGeneratorService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private static readonly JsonSerializerOptions JsonAiOptions = new() { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } };

    public async Task<ProgramTemplateResponse> GenerateProgramAsync(GenerateProgamRequest request, CancellationToken cancellationToken = default)
    {
        // 1. Get relevant exercise candidates using retrieval service
        var retrievalRequest = new ExerciseRetrievalRequest(
            AvailableEquipment: request.AvailableEquipment ?? [],
            MaxDifficulty: (LevelOfDifficulty)request.ExperienceLevel,
            ExcludedTags: request.Contraindications,
            Goal: request.Description,
            TargetFocus: request.Description
        );

        var candidateExercises = await exerciseRetrievalService.GetCandidateExercisesAsync(retrievalRequest, cancellationToken);

        if (!candidateExercises.Any())
        {
            throw new InvalidOperationException("No exercise definitions found matching the criteria.");
        }

        // 2. Build AI prompt with lean exercise definitions
        var prompt = BuildPrompt(request, candidateExercises);

        // 3. Call OpenAI API to generate program
        var aiResponse = await openAiClient.GenerateProgramTemplate(prompt, cancellationToken);

        // 4. Parse AI response into structured format`
        var programStructure = ParseAiResponse(aiResponse);

        // 5. Validate that all exercise IDs exist in our candidate set
        ValidateExerciseIds(programStructure, candidateExercises);

        // 6. Create ProgramTemplate entity
        var programTemplate = BuildProgramTemplate(request, programStructure);

        await programTemplateRepository.AddAsync(programTemplate);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(programTemplate);
    }

    private static string BuildPrompt(GenerateProgamRequest request, IReadOnlyList<ExercisePromptDto> availableExercises)
    {
        var exerciseJson = JsonSerializer.Serialize(availableExercises, JsonOptions);
        var experienceLevelString = GetExperienceLevelString(request);
        var fitnessGoalString = GetFitnessGoalString(request);

        return $@"
You are a highly experienced professional personal trainer and strength and conditioning coach. Generate a program using ONLY the allowed exercises.

USER REQUEST
- Program Name: {request.ProgramName}
- Experience Level: {experienceLevelString}
- Ensure the program is aligned with the Fitness Goal: {fitnessGoalString}
- Duration Weeks: The program must be only {request.DurationWeeks} weeks long.
- Workout Days (must include workout routines for all days): {string.Join(", ", request.WorkoutDayNames)}
- Target Focus: {request.Description}

CRITICAL CONSTRAINTS
- You MUST ONLY use exerciseDefinitionId values from ALLOWED_EXERCISE_IDS.
- If you are not sure an ID is allowed, DO NOT use it.
- Output MUST be valid JSON and MUST match the schema described below.
- Do NOT copy any example values from this prompt. Choose values based on the request + exercise metadata.
- isRestDay is always false.

AVAILABLE EXERCISES (for selection context):
{exerciseJson}

PROGRAMMING RULES
- Each workout day: 4–8 exercises depending on experience level.
- Order: compounds first, then accessories/isolation.
- Use balanced weekly volume across major muscle groups.
- Reps guidance:
  - Strength: 4–8 reps for main compounds, 6–12 for accessories.
  - Hypertrophy: 8–15 reps, isolation 12–20.
- Sets: 3–5 for most lifts (2–4 for isolation if needed).
- Rest seconds: 60–180 (compounds higher).
- RPE: 6–9.
- Progression algorithm across weeks:
  - Week 1: baseline.
  - Week 2: +1 set on 1–2 main compounds per day OR +1–2 reps per set (keep RPE same).
  - Week 3: keep volume, increase targetRpe by +1 on compounds (cap 9).
  - Week 4+: repeat the pattern by alternating volume and intensity changes.
  - If DurationWeeks < 3, apply only Week 1–2 rules.

FIELD RULES
- base the workkout day name based off the routine for that day e.g., 'push day', 'pull day', 'leg day', 'core day', 'upper body day', 'lower body day', 'full body day' or other appropriate name.
- targetWeight: set to null unless user-specific load data is provided (it is not). Do NOT guess.
- supersetGroupId and supersetRestSeconds: only use if you intentionally create a superset, otherwise null. Supersets should only be used for intermediate and experienced athletes. Supersets can ONLY be on the same workout day. To create a superset, set the supersetGroupId for 2 or more exercises on the same workout day to a new GUID.
- notes: optional, max 120 chars. Must be relevant to the exercise being performed.

REQUIRED OUTPUT JSON (no markdown, no commentary)
{{
  ""programName"": ""{request.ProgramName}"",
  ""description"": ""string"",
  ""weeks"": [
    {{
      ""weekNumber"": 1,
      ""notes"": ""string or null"",
      ""workoutDays"": [
        {{
          ""name"": ""string"",
          ""description"": ""string"",
          ""isRestDay"": false,
          ""exercises"": [
            {{
              ""exerciseDefinitionId"": 0,
              ""orderIndex"": 1,
              ""targetSets"": 0,
              ""targetReps"": 0,
              ""targetWeight"": null,
              ""restSeconds"": 0,
              ""targetRpe"": 0,
              ""supersetGroupId"": null,
              ""supersetRestSeconds"": null,
              ""notes"": null
            }}
          ]
        }}
      ]
    }}
  ]
}}

Now generate the complete program.
";
    }

    private static string GetExperienceLevelString(GenerateProgamRequest request)
    {
        return request.ExperienceLevel switch
        {
            ExperienceLevel.Beginner => "Beginner (less than 1 years experience)",
            ExperienceLevel.Intermediate => "Intermediate (1-3 years experience)",
            ExperienceLevel.Advanced => "Advanced (3+ years experience)",
            ExperienceLevel.Elite => "Elite (5+ years experience)",
            _ => throw new InvalidOperationException("Invalid experience level"),
        };
    }

    private static string GetFitnessGoalString(GenerateProgamRequest request)
    {
        return request.FitnessGoal switch
        {
            FitnessGoal.BuildMuscle => "Build Muscle (Hypertrophy):\n" +
                                       "- Goal: Maximise muscle growth via mechanical tension + sufficient volume.\n" +
                                       "- Target: grow tissue, not just feel tired.\n" +
                                       "- Sets per muscle group per week:\n" +
                                       "  - Beginner: 10-12\n" +
                                       "  - Intermediate: 12-20\n" +
                                       "  - Advanced-Elite: 22-25\n" +
                                       "- Reps: 6–12 (sometimes 12–15 for isolation).\n" +
                                       "- Weekly volume: ~10–20 hard sets per muscle group.\n" +
                                       "- Load: 60–80% 1RM.\n" +
                                       "- Rest: 60–120 sec.\n" +
                                       "- Reality check: If you’re not training close to failure (0–3 RIR), this won’t work. \"Feeling the burn\" doesn’t grow muscle—progressive overload does.",
            FitnessGoal.LoseWeight => "Lose Weight (Fat Loss):\n" +
                                      "- Goal: Maintain muscle, increase energy expenditure, create a calorie deficit.\n" +
                                      "- Reps: 8–15.\n" +
                                      "- Sets per muscle group per week:\n" +
                                      "  - Beginner: 6-8\n" +
                                      "  - Intermediate: 8-12\n" +
                                      "  - Advanced-Elite: 14\n" +
                                      "- Load: Moderate (challenging but repeatable).\n" +
                                      "- Rest: 30–90 sec.\n" +
                                      "- Training style: Circuits, supersets, full-body or upper/lower splits.\n" +
                                      "- Reality check: Fat loss is driven by diet. Training preserves muscle and increases output. If scale weight isn’t dropping, your nutrition is lying to you.",
            FitnessGoal.ImproveEndurance => "Improve Endurance (Muscular & Cardiovascular):\n" +
                                            "- Goal: Sustain effort over time, delay fatigue.\n" +
                                            "- Reps: 15–25+ or timed sets (30–90 sec).\n" +
                                            "- Sets per muscle group per week:\n" +
                                            "  - Beginner: 6-8\n" +
                                            "  - Intermediate: 8-10\n" +
                                            "  - Advanced-Elite: 10-12\n" +
                                            "- Load: Light–moderate.\n" +
                                            "- Rest: 15–60 sec.\n" +
                                            "- Reality check: If rest times creep up or loads creep too high, you’ve left endurance territory. Ego lifting kills conditioning.",
            FitnessGoal.IncreaseStrength => "Increase Strength:\n" +
                                            "- Goal: Maximise neural drive and force production.\n" +
                                            "- Reps: 1–5.\n" +
                                            "- Sets per muscle group per week:\n" +
                                            "  - Beginner: primary lifts: 6-8 sets, secondary lifts: 4-6 sets, 8-10 total.\n" +
                                            "  - Intermediate: primary lifts: 8-10 sets, secondary lifts: 4-8 sets, 10-12 total.\n" +
                                            "  - Advanced-Elite: primary lifts: 8-12 sets, secondary lifts: 6-8 sets, 12-15 total.\n" +
                                            "- Load: 80–95% 1RM.\n" +
                                            "- Rest: 2–5 min.\n" +
                                            "- Focus: Compound lifts, low fatigue, perfect technique.\n" +
                                            "- Reality check: Strength isn’t about \"feeling smashed.\" If you’re gassed, you’re training conditioning, not strength.",
            FitnessGoal.GeneralFitness => "General Fitness:\n" +
                                          "- Goal: Balanced health, function, and sustainability.\n" +
                                          "- Reps: 8–15.\n" +
                                          "- Sets: 6-10 sets.\n" +
                                          "- Load: Moderate.\n" +
                                          "- Rest: 60–90 sec.\n" +
                                          "- Reality check: This is maintenance territory. You’ll feel good—but you won’t maximise anything. That’s fine if that’s actually your goal.",
            _ => throw new InvalidOperationException("Invalid fitness goal"),
        };
    }

    private static AiProgramStructure ParseAiResponse(string aiResponse)
    {
        try
        {
            // Remove markdown code blocks if present
            var cleanJson = aiResponse.Trim();
            if (cleanJson.StartsWith("```json"))
            {
                cleanJson = cleanJson.Substring(7);
            }
            if (cleanJson.StartsWith("```"))
            {
                cleanJson = cleanJson.Substring(3);
            }
            if (cleanJson.EndsWith("```"))
            {
                cleanJson = cleanJson.Substring(0, cleanJson.Length - 3);
            }

            return JsonSerializer.Deserialize<AiProgramStructure>(cleanJson.Trim(), JsonAiOptions) ?? throw new InvalidOperationException("Failed to parse AI response.");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to parse AI response.", ex);
        }
    }

    private static void ValidateExerciseIds(AiProgramStructure programStructure, IReadOnlyList<ExercisePromptDto> availableExercises)
    {
        var availableIds = availableExercises.Select(e => e.Id).ToHashSet();
        var aiReturnedIds = programStructure.Weeks
            .SelectMany(w => w.WorkoutDays)
            .SelectMany(d => d.Exercises)
            .Select(e => e.ExerciseDefinitionId)
            .Distinct()
            .ToList();

        var invalidIds = aiReturnedIds.Where(id => !availableIds.Contains(id)).ToList();

        if (invalidIds.Any())
        {
            // Log to help diagnose: AI may be returning 0 (missing/wrong JSON key), or indices (1,2,3) instead of actual IDs
            Console.WriteLine("Available exercise IDs (from candidates): " + string.Join(", ", availableIds.OrderBy(x => x)));
            Console.WriteLine("AI-returned exerciseDefinitionId values: " + string.Join(", ", aiReturnedIds.OrderBy(x => x)));
            throw new InvalidOperationException(
                $"AI selected exercise IDs that don't exist in candidate set: {string.Join(", ", invalidIds)}. " +
                "Check that the AI response uses \"exerciseDefinitionId\" with values from the available list (not indices or 0).");
        }
    }

    private static ProgramTemplate BuildProgramTemplate(GenerateProgamRequest request, AiProgramStructure programStructure)
    {
        var template = new ProgramTemplate(
            Guid.NewGuid(),
            request.ProgramName,
            programStructure.Description ?? request.Description,
            request.DurationWeeks,
            request.ExperienceLevel,
            request.CreatedBy,
            false);

        foreach (var weekData in programStructure.Weeks.OrderBy(w => w.WeekNumber))
        {
            var week = template.AddWeek(weekData.WeekNumber, weekData.Notes);

            foreach (var dayData in weekData.WorkoutDays)
            {
                var workoutDay = new ProgramTemplateWorkoutDay
                {
                    Id = Guid.NewGuid(),
                    ProgramTemplateWeekId = week.Id,
                    Name = dayData.Name,
                    Description = dayData.Description,
                    IsRestDay = dayData.IsRestDay,
                    CreatedAt = DateTime.UtcNow
                };

                foreach (var exerciseData in dayData.Exercises.OrderBy(e => e.OrderIndex))
                {
                    var exercise = new ProgramTemplateWorkoutExercise
                    {
                        Id = Guid.NewGuid(),
                        ProgramTemplateWorkoutDayId = workoutDay.Id,
                        ExerciseDefinitionId = exerciseData.ExerciseDefinitionId,
                        OrderIndex = exerciseData.OrderIndex,
                        TargetSets = exerciseData.TargetSets ?? 0,
                        TargetReps = exerciseData.TargetReps ?? 0,
                        TargetWeight = exerciseData.TargetWeight,
                        TargetDurationSeconds = exerciseData.TargetDurationSeconds,
                        TargetDistance = exerciseData.TargetDistance,
                        RestSeconds = exerciseData.RestSeconds,
                        SupersetGroupId = exerciseData.SupersetGroupId,
                        SupersetRestSeconds = exerciseData.SupersetRestSeconds,
                        TargetRpe = exerciseData.TargetRpe,
                        Notes = exerciseData.Notes
                    };

                    workoutDay.Exercises.Add(exercise);
                }
                template.AddWorkoutDay(week.Id, workoutDay);
            }
        }

        return template;
    }

    private static ProgramTemplateResponse MapToDto(ProgramTemplate template)
    {
        return new ProgramTemplateResponse(
          Id: template.Id,
          Name: template.Name,
          Description: template.Description,
          DurationWeeks: template.DurationWeeks,
          ExperienceLevel: template.ExperienceLevel,
          IsActive: template.IsActive,
          CreatedAt: template.CreatedAt,
          UpdatedAt: template.UpdatedAt,
          Weeks: template.Weeks.Select(week => new ProgramTemplateWeekResponse(
            Id: week.Id,
            ProgramTemplateId: week.ProgramTemplateId,
            WeekNumber: week.WeekNumber,
            Notes: week.Notes,
            CreatedAt: week.CreatedAt,
            WorkoutDays: week.WorkoutDays.Select(day => new ProgramTemplateWorkoutDayResponse(
              Id: day.Id,
              ProgramTemplateWeekId: day.ProgramTemplateWeekId,
              Name: day.Name,
              Description: day.Description,
              IsRestDay: day.IsRestDay,
              CreatedAt: day.CreatedAt,
              Exercises: day.Exercises.Select(ex => new ProgramTemplateWorkoutExerciseResponse(
                Id: ex.Id,
                ProgramTemplateWorkoutDayId: ex.ProgramTemplateWorkoutDayId,
                ExerciseDefinitionId: ex.ExerciseDefinitionId,
                OrderIndex: ex.OrderIndex,
                Notes: ex.Notes,
                TargetSets: ex.TargetSets,
                TargetReps: ex.TargetReps,
                TargetWeight: ex.TargetWeight,
                TargetDurationSeconds: ex.TargetDurationSeconds,
                TargetDistance: ex.TargetDistance,
                RestSeconds: ex.RestSeconds,
                SupersetGroupId: ex.SupersetGroupId,
                SupersetRestSeconds: ex.SupersetRestSeconds,
                TargetRpe: ex.TargetRpe
              ))
            ))
          ))
        );
    }
}