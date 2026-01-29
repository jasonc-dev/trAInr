using System.Text.Json;
using System.Text.Json.Serialization;
using trAInr.Application.DTOs.AI;
using trAInr.Application.DTOs.ProgramTemplate;
using trAInr.Application.Interfaces;
using trAInr.Application.Interfaces.Repositories;
using trAInr.Application.Interfaces.Services.AI;
using trAInr.Domain.Aggregates;
using trAInr.Domain.Entities;

namespace trAInr.Application.Services.AI;

public class AiProgramGeneratorService : IAiProgramGeneratorService
{
    private readonly IOpenAiClient _openAiClient;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProgramTemplateRepository _programTemplateRepository;
    private readonly IExerciseDefinitionRepository _exerciseDefinitionRepository;
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
    private static readonly JsonSerializerOptions _jsonAiOptions = new() { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } };

    public AiProgramGeneratorService(IOpenAiClient openAiClient,
    IUnitOfWork unitOfWork,
    IProgramTemplateRepository programTemplateRepository, IExerciseDefinitionRepository exerciseDefinitionRepository)
    {
        _openAiClient = openAiClient;
        _unitOfWork = unitOfWork;
        _programTemplateRepository = programTemplateRepository;
        _exerciseDefinitionRepository = exerciseDefinitionRepository;
    }

    public async Task<ProgramTemplateResponse> GenerateProgramAsync(GenerateProgamRequest request, CancellationToken cancellationToken = default)
    {
        // 1. Get all available exercise definitions from DB
        var filteredExerciseDefinitions = (await _exerciseDefinitionRepository.GetAllAsync(cancellationToken))
            .Where(e => e.LevelOfDifficulty <= (LevelOfDifficulty)request.ExperienceLevel && e.Type != ExerciseType.Flexibility)
            .ToList();

        if (!filteredExerciseDefinitions.Any())
        {
            throw new InvalidOperationException("No exercise definitions found.");
        }

        // 2. Build AI prompt with available exercise definitions
        var prompt = BuildPrompt(request, filteredExerciseDefinitions);

        // 3. Call OpenAI API to generate program
        var aiResponse = await _openAiClient.GenerateProgramTemplate(prompt, cancellationToken);

        // 4. Parse AI response into structured format
        var programStructure = ParseAiResponse(aiResponse);

        // 5. Validate that all exercise IDs exist in our DB
        ValidateExerciseIds(programStructure, filteredExerciseDefinitions);

        // 6. Create ProgramTemplate entity
        var programTemplate = BuildProgramTemplate(request, programStructure);

        await _programTemplateRepository.AddAsync(programTemplate);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(programTemplate);
    }

    private static string BuildPrompt(GenerateProgamRequest request, IEnumerable<ExerciseDefinition> availableExercises)
    {
        var exerciseList = availableExercises.Select(e => new
        {
            e.Id,
            e.Name,
            e.MovementPattern,
            e.PrimaryMuscleGroup,
            e.SecondaryMuscleGroup,
            e.LevelOfDifficulty
        });

        var exerciseJson = JsonSerializer.Serialize(exerciseList, _jsonOptions);

        return $@"You are a professional fitness trainer creating a structured exercise program.

USER REQUEST:
- Program Name: {request.ProgramName}
- Experience Level: {request.ExperienceLevel}
- Duration: {request.DurationWeeks} weeks
- Workout Days per Week: {string.Join(", ", request.WorkoutDayNames)}
- Target Focus: {request.Description}

AVAILABLE EXERCISES (YOU MUST ONLY USE EXERCISES FROM THIS LIST):
{exerciseJson}. Check the 'Id' exists in the list before selecting any exercises.

INSTRUCTIONS:
1. Create a {request.DurationWeeks}-week program with the specified workout days
2. CRITICAL: You can ONLY use ExerciseDefinitionId values from the provided list above
3. For each workout day, select 4-8 exercises appropriate for that day's focus
4. Include proper progression across weeks (increasing sets, reps, or intensity)
5. Assign appropriate sets (3-5), reps (6-15 for strength, 12-20 for hypertrophy), rest times (60-180 seconds), and RPE (6-9)
6. Focus on appropriate structure for an exercise day, that is compound exercises before isolation exercises.
7. CRITICAL: For rest days (isRestDay: true), the exercises array MUST be empty [] - do not include any exercises on rest days
8. CRITICAL: For workout days (isRestDay: false), ALL exercises MUST have non-null values for targetSets and targetReps - these are required fields

RESPONSE FORMAT (JSON):
{{
  ""programName"": ""string"",
  ""description"": ""string"",
  ""weeks"": [
    {{
      ""weekNumber"": 1,
      ""notes"": ""Week 1 - Foundation"",
      ""workoutDays"": [
        {{
          ""name"": ""Push Day"",
          ""description"": ""Chest, Shoulders, Triceps"",
          ""isRestDay"": false,
          ""exercises"": [
            {{
              ""exerciseDefinitionId"": ""int-from-available-list"",
              ""orderIndex"": 1,
              ""targetSets"": 4,
              ""targetReps"": 8,
              ""targetWeight"": 50,
              ""restSeconds"": 120,
              ""targetRpe"": 7,
              ""supersetGroupId"": null,
              ""supersetRestSeconds"": null,
              ""notes"": ""Warm up properly""
            }}
          ]
        }}
      ]
    }}
  ]
}}

Generate the complete program now:";
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

            return JsonSerializer.Deserialize<AiProgramStructure>(cleanJson.Trim(), _jsonAiOptions) ?? throw new InvalidOperationException("Failed to parse AI response.");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to parse AI response.", ex);
        }
    }

    private static void ValidateExerciseIds(AiProgramStructure programStructure, IEnumerable<ExerciseDefinition> availableExercises)
    {
        var availableIds = availableExercises.Select(e => e.Id).ToHashSet();
        var userIds = programStructure.Weeks
          .SelectMany(w => w.WorkoutDays)
          .SelectMany(d => d.Exercises)
          .Select(e => e.ExerciseDefinitionId)
          .Distinct();

        var invalidIds = userIds.Where(id => !availableIds.Contains(id)).ToList();

        if (invalidIds.Any())
        {
            throw new InvalidOperationException($"AI selected exercise IDs that don't exist in database: {string.Join(", ", invalidIds)}");
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