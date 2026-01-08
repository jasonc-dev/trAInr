using Microsoft.Extensions.Logging;
using trAInr.Application.DTOs;
using trAInr.Application.Interfaces;
using trAInr.Application.Interfaces.Repositories;
using trAInr.Application.Interfaces.Services;
using trAInr.Domain.Aggregates;
using trAInr.Domain.Entities;

namespace trAInr.Application.Services;

public class AssignedProgrammeService(
    IAssignedProgramRepository assignedProgramRepository,
    IAthleteRepository athleteRepository,
    IUnitOfWork unitOfWork,
    ILogger<AssignedProgrammeService> logger)
    : IAssignedProgrammeService
{
    public async Task<ProgrammeResponse?> GetByIdAsync(Guid id)
    {
        var assignedProgram = await assignedProgramRepository.GetByIdAsync(id);
        return assignedProgram is null ? null : MapToResponse(assignedProgram);
    }

    public async Task<IEnumerable<ProgrammeSummaryResponse>> GetByAthleteIdAsync(Guid athleteId)
    {
        // userId is actually athleteId in the domain model
        var assignedPrograms = await assignedProgramRepository.GetByAthleteIdAsync(athleteId);
        return assignedPrograms.Select(MapToSummary);
    }

    public async Task<ProgrammeSummaryResponse?> GetActiveByAthleteIdAsync(Guid athleteId)
    {
        // userId is actually athleteId in the domain model
        var assignedProgram = await assignedProgramRepository.GetActiveByAthleteIdAsync(athleteId);
        return assignedProgram is null ? null : MapToSummary(assignedProgram);
    }

    public async Task<ProgrammeResponse> CreateAsync(Guid userId, CreateProgrammeRequest request)
    {
        // userId is actually athleteId in the domain model. Check variable assignment.
        var athlete = await athleteRepository.GetByIdAsync(userId) ??
        throw new InvalidOperationException($"Athlete with ID {userId} not found");

        // Note: AssignedProgram requires a ProgramTemplateId. For custom programs, we use Guid.Empty
        // This may need to be adjusted based on your business logic
        var assignedProgram = new AssignedProgram(
                Guid.NewGuid(),
                athlete.Id, // athleteId
                Guid.Empty, // ProgramTemplateId - using Empty for custom programs
                request.Name,
                request.Description,
                request.DurationWeeks,
                request.StartDate);

        // Automatically create weeks based on durationWeeks
        for (int weekNumber = 1; weekNumber <= request.DurationWeeks; weekNumber++)
        {
            assignedProgram.AddWeek(weekNumber, null);
        }

        await assignedProgramRepository.AddAsync(assignedProgram);
        await unitOfWork.SaveChangesAsync();
        return MapToResponse(assignedProgram);
    }

    public async Task<ProgrammeResponse?> UpdateAsync(Guid id, UpdateProgrammeRequest request)
    {
        var assignedProgram = await assignedProgramRepository.GetByIdAsync(id);
        if (assignedProgram is null) return null;

        // Use domain methods where applicable
        if (request.IsActive && !assignedProgram.IsActive)
        {
            assignedProgram.Activate();
        }
        else if (!request.IsActive && assignedProgram.IsActive)
        {
            assignedProgram.Deactivate();
        }

        // Note: AssignedProgram doesn't have public setters for Name/Description
        // This may require adding domain methods or using reflection/EF Core change tracking
        // For now, we'll need to update through EF Core's change tracking
        // This is a limitation that should be addressed with proper domain methods
        await assignedProgramRepository.UpdateAsync(assignedProgram);
        await unitOfWork.SaveChangesAsync();
        return MapToResponse(assignedProgram);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var assignedProgram = await assignedProgramRepository.GetByIdAsync(id);
        if (assignedProgram is null) return false;

        await assignedProgramRepository.DeleteAsync(assignedProgram);
        await unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<ProgrammeWeekResponse?> AddWeekAsync(Guid programmeId, CreateProgrammeWeekRequest request)
    {
        var assignedProgram = await assignedProgramRepository.GetByIdAsync(programmeId);
        if (assignedProgram is null) return null;

        try
        {
            var week = assignedProgram.AddWeek(request.WeekNumber, request.Notes);
            // Explicitly add so EF Core tracks it as Added
            await assignedProgramRepository.AddProgrammeWeekAsync(week);
            await unitOfWork.SaveChangesAsync();
            return MapWeekToResponse(week);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Failed to add week to program {ProgrammeId}", programmeId);
            return null;
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Failed to add week to program {ProgrammeId}", programmeId);
            return null;
        }
    }

    public async Task<ProgrammeWeekResponse?> UpdateWeekAsync(Guid weekId, UpdateProgrammeWeekRequest request)
    {
        var assignedProgram = await assignedProgramRepository.GetByWeekIdAsync(weekId);
        if (assignedProgram is null) return null;

        var updatedWeek = assignedProgram.UpdateWeek(weekId, request.IsCompleted);
        if (updatedWeek is null) return null;

        await assignedProgramRepository.UpdateAsync(assignedProgram);
        await unitOfWork.SaveChangesAsync();
        return MapWeekToResponse(updatedWeek);
    }

    public Task<IEnumerable<ProgrammeSummaryResponse>> GetPreMadeProgrammesAsync()
    {
        // Note: Pre-made programmes would be ProgramTemplates, not AssignedPrograms
        // This requires a ProgramTemplate repository which doesn't exist yet
        // For now, returning empty list
        logger.LogWarning("GetPreMadeProgrammesAsync is not yet implemented - requires ProgramTemplate repository");
        return Task.FromResult(Enumerable.Empty<ProgrammeSummaryResponse>());
    }

    public async Task<ProgrammeResponse?> CloneProgrammeAsync(Guid programmeId, Guid userId)
    {
        // userId is actually athleteId in the domain model
        var sourceAssignedProgram = await assignedProgramRepository.GetByIdAsync(programmeId);
        if (sourceAssignedProgram is null) return null;

        var athlete = await athleteRepository.GetByIdAsync(userId);
        if (athlete is null) throw new InvalidOperationException($"Athlete with ID {userId} not found");

        // Clone the AssignedProgram
        var clonedAssignedProgram = new AssignedProgram(
            Guid.NewGuid(),
            userId, // athleteId
            sourceAssignedProgram.ProgramTemplateId, // Use same template ID
            $"{sourceAssignedProgram.Name} (Copy)",
            sourceAssignedProgram.Description,
            sourceAssignedProgram.DurationWeeks,
            DateOnly.FromDateTime(DateTime.UtcNow));

        // Deactivate the cloned program initially
        clonedAssignedProgram.Deactivate();

        await assignedProgramRepository.AddAsync(clonedAssignedProgram);
        await unitOfWork.SaveChangesAsync();

        // Clone weeks from source program
        foreach (var sourceWeek in sourceAssignedProgram.Weeks.OrderBy(w => w.WeekNumber))
        {
            var clonedWeek = clonedAssignedProgram.AddWeek(sourceWeek.WeekNumber, sourceWeek.Notes);
            clonedWeek.IsCompleted = false; // Reset completion status for cloned week
            // Explicitly add so EF Core tracks it as Added
            await assignedProgramRepository.AddProgrammeWeekAsync(clonedWeek);
        }

        await unitOfWork.SaveChangesAsync();

        return MapToResponse(clonedAssignedProgram);
    }

    private static ProgrammeResponse MapToResponse(AssignedProgram assignedProgram)
    {
        return new ProgrammeResponse(
            assignedProgram.Id,
            assignedProgram.AthleteId, // This maps to UserId in the DTO
            assignedProgram.Name,
            assignedProgram.Description,
            assignedProgram.DurationWeeks,
            assignedProgram.ProgramTemplateId != Guid.Empty, // IsPreMade
            assignedProgram.IsActive,
            assignedProgram.StartDate,
            assignedProgram.EndDate,
            assignedProgram.CreatedAt,
            assignedProgram.Weeks.Select(MapWeekToResponse).OrderBy(w => w.WeekNumber));
    }

    private static ProgrammeWeekResponse MapWeekToResponse(ProgrammeWeek week)
    {
        return new ProgrammeWeekResponse(
            week.Id,
            week.WeekNumber,
            week.Notes,
            week.IsCompleted,
            week.WorkoutDays.Select(MapWorkoutDayToResponse));
    }

    private static WorkoutDayResponse MapWorkoutDayToResponse(WorkoutDay day)
    {
        return new WorkoutDayResponse(
            day.Id,
            day.ProgrammeWeekId,
            day.DayOfWeek,
            day.Name,
            day.Description,
            day.ScheduledDate,
            day.CompletedDate,
            day.IsCompleted,
            day.IsRestDay,
            day.Exercises.Select(MapWorkoutExerciseToResponse));
    }

    private static WorkoutExerciseResponse MapWorkoutExerciseToResponse(WorkoutExercise exercise)
    {
        return new WorkoutExerciseResponse(
            exercise.Id,
            exercise.ExerciseDefinitionId,
            exercise.ExerciseDefinition.Name,
            exercise.OrderIndex,
            exercise.Notes,
            exercise.TargetSets,
            exercise.TargetReps,
            exercise.TargetWeight,
            exercise.TargetDurationSeconds,
            exercise.TargetDistance,
            exercise.RestSeconds,
            exercise.TargetRpe,
            exercise.SupersetGroupId,
            exercise.SupersetRestSeconds,
            exercise.Sets.Select(MapExerciseSetToResponse));
    }

    private static ExerciseSetResponse MapExerciseSetToResponse(ExerciseSet set)
    {
#pragma warning disable CS0618 // Type or member is obsolete
        var isWarmup = set.IsWarmup; // Keep for backward compatibility
#pragma warning restore CS0618 // Type or member is obsolete
        return new ExerciseSetResponse(
            set.Id,
            set.SetNumber,
            set.Reps,
            set.Weight,
            set.DurationSeconds,
            set.Distance,
            set.Difficulty,
            set.Intensity,
            set.IsCompleted,
            isWarmup,
            set.SetType,
            set.DropPercentage,
            set.Notes,
            set.CompletedAt);
    }

    public async Task<ProgrammeWeekResponse?> CopyWeekAsync(Guid sourceWeekId, int targetWeekNumber)
    {
        var assignedProgram = await assignedProgramRepository.GetByWeekIdAsync(sourceWeekId);
        if (assignedProgram is null) return null;

        var sourceWeek = assignedProgram.GetWeekById(sourceWeekId);
        if (sourceWeek is null) return null;

        // Validate target week number
        if (targetWeekNumber < 1)
        {
            logger.LogWarning("Invalid target week number {TargetWeekNumber}", targetWeekNumber);
            return null;
        }

        // Check if target week already exists
        var existingTargetWeek = assignedProgram.GetWeekByNumber(targetWeekNumber);
        if (existingTargetWeek is not null)
        {
            logger.LogWarning("Week {WeekNumber} already exists in program {ProgrammeId}", targetWeekNumber, assignedProgram.Id);
            return null;
        }

        try
        {
            // Create the new week (not completed)
            var newWeek = assignedProgram.AddWeek(targetWeekNumber, sourceWeek.Notes);
            await assignedProgramRepository.AddProgrammeWeekAsync(newWeek);

            // Copy workout days
            foreach (var sourceDay in sourceWeek.WorkoutDays)
            {
                var newDay = new WorkoutDay
                {
                    Id = Guid.NewGuid(),
                    ProgrammeWeekId = newWeek.Id,
                    DayOfWeek = sourceDay.DayOfWeek,
                    Name = sourceDay.Name,
                    Description = sourceDay.Description,
                    IsRestDay = sourceDay.IsRestDay,
                    IsCompleted = false, // Reset completion status
                    CompletedDate = null,
                    CreatedAt = DateTime.UtcNow
                };

                newWeek.WorkoutDays.Add(newDay);
                await assignedProgramRepository.AddWorkoutDayAsync(newDay);

                // Copy exercises
                foreach (var sourceExercise in sourceDay.Exercises)
                {
                    var newExercise = new WorkoutExercise
                    {
                        Id = Guid.NewGuid(),
                        WorkoutDayId = newDay.Id,
                        ExerciseDefinitionId = sourceExercise.ExerciseDefinitionId,
                        OrderIndex = sourceExercise.OrderIndex,
                        Notes = sourceExercise.Notes,
                        TargetSets = sourceExercise.TargetSets,
                        TargetReps = sourceExercise.TargetReps,
                        TargetWeight = sourceExercise.TargetWeight,
                        TargetDurationSeconds = sourceExercise.TargetDurationSeconds,
                        TargetDistance = sourceExercise.TargetDistance,
                        RestSeconds = sourceExercise.RestSeconds,
                        TargetRpe = sourceExercise.TargetRpe,
                        CreatedAt = DateTime.UtcNow
                    };

                    newDay.Exercises.Add(newExercise);
                    await assignedProgramRepository.AddWorkoutExerciseAsync(newExercise);

                    // Copy sets (but reset completion status)
                    foreach (var sourceSet in sourceExercise.Sets)
                    {
#pragma warning disable CS0618
                        var isWarmup = sourceSet.IsWarmup;
#pragma warning restore CS0618
                        var newSet = new ExerciseSet
                        {
                            Id = Guid.NewGuid(),
                            WorkoutExerciseId = newExercise.Id,
                            SetNumber = sourceSet.SetNumber,
                            Reps = sourceSet.Reps,
                            Weight = sourceSet.Weight,
                            DurationSeconds = sourceSet.DurationSeconds,
                            Distance = sourceSet.Distance,
                            Difficulty = sourceSet.Difficulty,
                            Intensity = sourceSet.Intensity,
                            IsCompleted = false, // Reset completion status
                            IsWarmup = isWarmup,
                            SetType = sourceSet.SetType,
                            DropPercentage = sourceSet.DropPercentage,
                            Notes = sourceSet.Notes,
                            CompletedAt = null, // Reset completion timestamp
                            CreatedAt = DateTime.UtcNow
                        };

                        newExercise.Sets.Add(newSet);
                        await assignedProgramRepository.AddExerciseSetAsync(newSet);
                    }
                }
            }

            await unitOfWork.SaveChangesAsync();
            
            // Reload the program to get the full response with navigation properties
            var updatedProgram = await assignedProgramRepository.GetByIdAsync(assignedProgram.Id);
            var createdWeek = updatedProgram?.GetWeekByNumber(targetWeekNumber);
            
            return createdWeek is not null ? MapWeekToResponse(createdWeek) : null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to copy week {SourceWeekId} to week {TargetWeekNumber}", sourceWeekId, targetWeekNumber);
            return null;
        }
    }

    public async Task<ProgrammeWeekResponse?> CopyWeekContentAsync(Guid sourceWeekId, Guid targetWeekId)
    {
        // Get the program containing the source week
        var assignedProgram = await assignedProgramRepository.GetByWeekIdAsync(sourceWeekId);
        if (assignedProgram is null) return null;

        var sourceWeek = assignedProgram.GetWeekById(sourceWeekId);
        if (sourceWeek is null) return null;

        var targetWeek = assignedProgram.GetWeekById(targetWeekId);
        if (targetWeek is null)
        {
            logger.LogWarning("Target week {TargetWeekId} not found in program {ProgrammeId}", targetWeekId, assignedProgram.Id);
            return null;
        }

        // Ensure source and target are different
        if (sourceWeekId == targetWeekId)
        {
            logger.LogWarning("Cannot copy week to itself");
            return null;
        }

        try
        {
            // Copy workout days from source to target
            foreach (var sourceDay in sourceWeek.WorkoutDays)
            {
                var newDay = new WorkoutDay
                {
                    Id = Guid.NewGuid(),
                    ProgrammeWeekId = targetWeek.Id,
                    DayOfWeek = sourceDay.DayOfWeek,
                    Name = sourceDay.Name,
                    Description = sourceDay.Description,
                    IsRestDay = sourceDay.IsRestDay,
                    IsCompleted = false, // Reset completion status
                    CompletedDate = null,
                    CreatedAt = DateTime.UtcNow
                };

                targetWeek.WorkoutDays.Add(newDay);
                await assignedProgramRepository.AddWorkoutDayAsync(newDay);

                // Copy exercises
                foreach (var sourceExercise in sourceDay.Exercises)
                {
                    var newExercise = new WorkoutExercise
                    {
                        Id = Guid.NewGuid(),
                        WorkoutDayId = newDay.Id,
                        ExerciseDefinitionId = sourceExercise.ExerciseDefinitionId,
                        OrderIndex = sourceExercise.OrderIndex,
                        Notes = sourceExercise.Notes,
                        TargetSets = sourceExercise.TargetSets,
                        TargetReps = sourceExercise.TargetReps,
                        TargetWeight = sourceExercise.TargetWeight,
                        TargetDurationSeconds = sourceExercise.TargetDurationSeconds,
                        TargetDistance = sourceExercise.TargetDistance,
                        RestSeconds = sourceExercise.RestSeconds,
                        TargetRpe = sourceExercise.TargetRpe,
                        CreatedAt = DateTime.UtcNow
                    };

                    newDay.Exercises.Add(newExercise);
                    await assignedProgramRepository.AddWorkoutExerciseAsync(newExercise);

                    // Copy sets (but reset completion status)
                    foreach (var sourceSet in sourceExercise.Sets)
                    {
#pragma warning disable CS0618
                        var isWarmup = sourceSet.IsWarmup;
#pragma warning restore CS0618
                        var newSet = new ExerciseSet
                        {
                            Id = Guid.NewGuid(),
                            WorkoutExerciseId = newExercise.Id,
                            SetNumber = sourceSet.SetNumber,
                            Reps = sourceSet.Reps,
                            Weight = sourceSet.Weight,
                            DurationSeconds = sourceSet.DurationSeconds,
                            Distance = sourceSet.Distance,
                            Difficulty = sourceSet.Difficulty,
                            Intensity = sourceSet.Intensity,
                            IsCompleted = false, // Reset completion status
                            IsWarmup = isWarmup,
                            SetType = sourceSet.SetType,
                            DropPercentage = sourceSet.DropPercentage,
                            Notes = sourceSet.Notes,
                            CompletedAt = null, // Reset completion timestamp
                            CreatedAt = DateTime.UtcNow
                        };

                        newExercise.Sets.Add(newSet);
                        await assignedProgramRepository.AddExerciseSetAsync(newSet);
                    }
                }
            }

            await unitOfWork.SaveChangesAsync();

            // Reload the program to get the full response with navigation properties
            var updatedProgram = await assignedProgramRepository.GetByIdAsync(assignedProgram.Id);
            var updatedWeek = updatedProgram?.GetWeekById(targetWeekId);

            return updatedWeek is not null ? MapWeekToResponse(updatedWeek) : null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to copy week content from {SourceWeekId} to {TargetWeekId}", sourceWeekId, targetWeekId);
            return null;
        }
    }

    private static ProgrammeSummaryResponse MapToSummary(AssignedProgram assignedProgram)
    {
        var completedWeeks = assignedProgram.Weeks.Count(w => w.IsCompleted);
        var progressPercentage = assignedProgram.DurationWeeks > 0
            ? (double)completedWeeks / assignedProgram.DurationWeeks * 100
            : 0;

        return new ProgrammeSummaryResponse(
            assignedProgram.Id,
            assignedProgram.Name,
            assignedProgram.Description,
            assignedProgram.DurationWeeks,
            assignedProgram.IsActive,
            assignedProgram.StartDate,
            completedWeeks,
            progressPercentage);
    }
}