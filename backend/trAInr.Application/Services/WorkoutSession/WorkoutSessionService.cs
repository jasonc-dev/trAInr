using trAInr.Application.DTOs;
using trAInr.Application.Interfaces;
using trAInr.Application.Interfaces.Repositories;
using trAInr.Application.Interfaces.Services;
using trAInr.Domain.Aggregates;
using trAInr.Domain.Entities;

namespace trAInr.Application.Services.WorkoutSession;

/// <summary>
///     Service for managing workout days, exercises, and sets within assigned programs.
///     Works through the AssignedProgram aggregate root to maintain DDD boundaries.
/// </summary>
public class WorkoutSessionService(
    IAssignedProgramRepository assignedProgramRepository,
    IExerciseDefinitionRepository exerciseDefinitionRepository,
    IUnitOfWork unitOfWork)
    : IWorkoutSessionService
{
    #region Workout Days

    public async Task<WorkoutDayResponse?> GetWorkoutDayAsync(Guid workoutDayId)
    {
        var assignedProgram = await assignedProgramRepository.GetByWorkoutDayIdAsync(workoutDayId);
        if (assignedProgram is null) return null;

        var workoutDay = assignedProgram.Weeks
            .SelectMany(w => w.WorkoutDays)
            .FirstOrDefault(d => d.Id == workoutDayId);

        return workoutDay is null ? null : MapWorkoutDayToResponse(workoutDay);
    }

    public async Task<ProgrammeWeekResponse?> CreateWorkoutDayAsync(Guid weekId, CreateWorkoutDayRequest request)
    {
        var assignedProgram = await assignedProgramRepository.GetByWeekIdAsync(weekId);
        if (assignedProgram is null) return null;

        var week = assignedProgram.GetWeekById(weekId);
        if (week is null) return null;

        DateOnly? scheduledDate = request.ScheduledDate is not null
                ? new DateOnly(request.ScheduledDate.Value.Year, request.ScheduledDate.Value.Month, request.ScheduledDate.Value.Day)
                : null;

        var workoutDay = new WorkoutDay
        {
            ScheduledDate = scheduledDate,
            Name = request.Name,
            Description = request.Description,
            IsRestDay = request.IsRestDay,
        };

        // Add the workout day to the week via the aggregate root
        var addedWorkoutDay = assignedProgram.AddWorkoutDay(weekId, workoutDay);
        if (addedWorkoutDay is null) return null;

        // Explicitly add the new WorkoutDay so EF tracks it as Added
        await assignedProgramRepository.AddWorkoutDayAsync(addedWorkoutDay);
        await unitOfWork.SaveChangesAsync();

        return MapWeekToResponse(week);
    }

    public async Task<ProgrammeWeekResponse?> UpdateWorkoutDayAsync(Guid workoutDayId, UpdateWorkoutDayRequest request)
    {
        AssignedProgram? assignedProgram = await assignedProgramRepository.GetByWorkoutDayIdAsync(workoutDayId);
        if (assignedProgram is null) return null;

        WorkoutDay? workoutDay = assignedProgram.Weeks
            .SelectMany(w => w.WorkoutDays)
            .FirstOrDefault(d => d.Id == workoutDayId);

        if (workoutDay is null) return null;

        DateOnly? scheduledDate = request.ScheduledDate is not null
            ? new DateOnly(request.ScheduledDate.Value.Year, request.ScheduledDate.Value.Month, request.ScheduledDate.Value.Day)
            : null;

        workoutDay.Name = request.Name;
        workoutDay.Description = request.Description;
        workoutDay.ScheduledDate = scheduledDate;
        workoutDay.IsCompleted = request.IsCompleted;
        workoutDay.IsRestDay = request.IsRestDay;

        var week = assignedProgram.Weeks.FirstOrDefault(w => w.WorkoutDays.Any(d => d.Id == workoutDayId));
        if (week is null) return null;

        await assignedProgramRepository.UpdateAsync(assignedProgram);
        await unitOfWork.SaveChangesAsync();

        return MapWeekToResponse(week);
    }

    public async Task<ProgrammeWeekResponse?> DeleteWorkoutDayAsync(Guid workoutDayId)
    {
        var assignedProgram = await assignedProgramRepository.GetByWorkoutDayIdAsync(workoutDayId);
        if (assignedProgram is null) return null;

        var week = assignedProgram.Weeks
            .FirstOrDefault(w => w.WorkoutDays.Any(d => d.Id == workoutDayId));

        if (week is null) return null;

        var workoutDay = week.WorkoutDays.FirstOrDefault(d => d.Id == workoutDayId);
        if (workoutDay is null) return null;

        week.WorkoutDays.Remove(workoutDay);
        await assignedProgramRepository.UpdateAsync(assignedProgram);
        await unitOfWork.SaveChangesAsync();
        return MapWeekToResponse(week);
    }

    public async Task<ProgrammeWeekResponse?> CompleteWorkoutAsync(Guid workoutDayId, CompleteWorkoutRequest request)
    {
        var assignedProgram = await assignedProgramRepository.GetByWorkoutDayIdAsync(workoutDayId);
        if (assignedProgram is null) return null;

        var workoutDay = assignedProgram.Weeks
            .SelectMany(w => w.WorkoutDays)
            .FirstOrDefault(d => d.Id == workoutDayId);

        if (workoutDay is null) return null;

        workoutDay.Complete(request.CompletedAt);

        var week = assignedProgram.Weeks.FirstOrDefault(w => w.WorkoutDays.Any(d => d.Id == workoutDayId));
        if (week is null) return null;

        await assignedProgramRepository.UpdateAsync(assignedProgram);
        await unitOfWork.SaveChangesAsync();

        return MapWeekToResponse(week);
    }

    #endregion

    #region Workout Exercises

    public async Task<WorkoutDayResponse?> AddExerciseToWorkoutAsync(Guid workoutDayId,
        AddWorkoutExerciseRequest request)
    {
        var assignedProgram = await assignedProgramRepository.GetByWorkoutDayIdAsync(workoutDayId);
        if (assignedProgram is null) return null;

        var workoutDay = assignedProgram.Weeks
            .SelectMany(w => w.WorkoutDays)
            .FirstOrDefault(d => d.Id == workoutDayId);

        if (workoutDay is null) return null;

        // Verify exercise exists
        var exerciseDefinition = await exerciseDefinitionRepository.GetByIdAsync(request.ExerciseDefinitionId);
        if (exerciseDefinition is null) return null;

        var workoutExercise = workoutDay.AddExercise(
            request.ExerciseDefinitionId,
            request.OrderIndex,
            request.TargetSets,
            request.TargetReps,
            request.TargetWeight,
            request.TargetDurationSeconds,
            request.TargetDistance,
            request.RestSeconds,
            request.TargetRpe,
            request.Notes);

        // Explicitly add so EF tracks as Added
        await assignedProgramRepository.AddWorkoutExerciseAsync(workoutExercise);
        await unitOfWork.SaveChangesAsync();

        return MapWorkoutDayToResponse(workoutDay);
    }

    public async Task<WorkoutDayResponse?> UpdateWorkoutExerciseAsync(Guid workoutExerciseId,
        UpdateWorkoutExerciseRequest request)
    {
        var assignedProgram = await assignedProgramRepository.GetByWorkoutExerciseIdAsync(workoutExerciseId);
        if (assignedProgram is null) return null;

        var workoutExercise = assignedProgram.Weeks
            .SelectMany(w => w.WorkoutDays)
            .SelectMany(d => d.Exercises)
            .FirstOrDefault(e => e.Id == workoutExerciseId);

        if (workoutExercise is null) return null;

        var workoutDay = assignedProgram.Weeks
            .SelectMany(w => w.WorkoutDays)
            .FirstOrDefault(d => d.Exercises.Any(e => e.Id == workoutExerciseId));

        if (workoutDay is null) return null;

        workoutExercise.OrderIndex = request.OrderIndex;
        workoutExercise.Notes = request.Notes;
        workoutExercise.TargetSets = request.TargetSets;
        workoutExercise.TargetReps = request.TargetReps;
        workoutExercise.TargetWeight = request.TargetWeight;
        workoutExercise.TargetDurationSeconds = request.TargetDurationSeconds;
        workoutExercise.TargetDistance = request.TargetDistance;
        workoutExercise.RestSeconds = request.RestSeconds;
        workoutExercise.TargetRpe = request.TargetRpe;

        await assignedProgramRepository.UpdateAsync(assignedProgram);
        await unitOfWork.SaveChangesAsync();

        return MapWorkoutDayToResponse(workoutDay);
    }

    public async Task<WorkoutDayResponse?> RemoveExerciseFromWorkoutAsync(Guid workoutExerciseId)
    {
        var assignedProgram = await assignedProgramRepository.GetByWorkoutExerciseIdAsync(workoutExerciseId);
        if (assignedProgram is null) return null;

        var workoutDay = assignedProgram.Weeks
            .SelectMany(w => w.WorkoutDays)
            .FirstOrDefault(d => d.Exercises.Any(e => e.Id == workoutExerciseId));

        if (workoutDay is null) return null;

        var removed = workoutDay.RemoveExercise(workoutExerciseId);
        if (!removed) return null;

        await assignedProgramRepository.UpdateAsync(assignedProgram);
        await unitOfWork.SaveChangesAsync();

        return MapWorkoutDayToResponse(workoutDay);
    }

    public async Task<WorkoutDayResponse?> ReorderExercisesAsync(Guid workoutDayId, List<Guid> workoutExerciseIds)
    {
        var assignedProgram = await assignedProgramRepository.GetByWorkoutDayIdAsync(workoutDayId);
        if (assignedProgram is null) return null;

        var workoutDay = assignedProgram.Weeks
            .SelectMany(w => w.WorkoutDays)
            .FirstOrDefault(d => d.Id == workoutDayId);

        if (workoutDay is null) return null;

        var reordered = workoutDay.ReorderExercises(workoutExerciseIds);
        if (!reordered) return null;

        await assignedProgramRepository.UpdateAsync(assignedProgram);
        await unitOfWork.SaveChangesAsync();

        return MapWorkoutDayToResponse(workoutDay);
    }

    #endregion

    #region Exercise Sets

    public async Task<WorkoutExerciseResponse?> AddSetAsync(Guid workoutExerciseId, CreateExerciseSetRequest request)
    {
        var assignedProgram = await assignedProgramRepository.GetByWorkoutExerciseIdAsync(workoutExerciseId);
        if (assignedProgram is null) return null;

        var workoutExercise = assignedProgram.Weeks
            .SelectMany(w => w.WorkoutDays)
            .SelectMany(d => d.Exercises)
            .FirstOrDefault(e => e.Id == workoutExerciseId);

        if (workoutExercise is null) return null;

        var exerciseSet = new ExerciseSet
        {
            Id = Guid.NewGuid(),
            WorkoutExerciseId = workoutExerciseId,
            SetNumber = request.SetNumber,
            Reps = request.Reps,
            Weight = request.Weight,
            DurationSeconds = request.DurationSeconds,
            Distance = request.Distance,
            Difficulty = request.Difficulty,
            Intensity = request.Intensity,
            SetType = request.SetType,
            DropPercentage = request.DropPercentage,
            Notes = request.Notes,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        workoutExercise.Sets.Add(exerciseSet);

        // Explicitly add so EF Core tracks it as Added
        await assignedProgramRepository.AddExerciseSetAsync(exerciseSet);
        await unitOfWork.SaveChangesAsync();

        return MapWorkoutExerciseToResponse(workoutExercise);
    }

    public async Task<WorkoutExerciseResponse?> UpdateSetAsync(Guid setId, UpdateExerciseSetRequest request)
    {
        var assignedProgram = await assignedProgramRepository.GetByExerciseSetIdAsync(setId);
        if (assignedProgram is null) return null;

        var exerciseSet = assignedProgram.Weeks
            .SelectMany(w => w.WorkoutDays)
            .SelectMany(d => d.Exercises)
            .SelectMany(e => e.Sets)
            .FirstOrDefault(s => s.Id == setId);

        if (exerciseSet is null) return null;

        var workoutExercise = assignedProgram.Weeks
            .SelectMany(w => w.WorkoutDays)
            .SelectMany(d => d.Exercises)
            .FirstOrDefault(e => e.Sets.Any(s => s.Id == setId));

        if (workoutExercise is null) return null;

        exerciseSet.Reps = request.Reps;
        exerciseSet.Weight = request.Weight;
        exerciseSet.DurationSeconds = request.DurationSeconds;
        exerciseSet.Distance = request.Distance;
        exerciseSet.Difficulty = request.Difficulty;
        exerciseSet.Intensity = request.Intensity;
        exerciseSet.IsCompleted = request.IsCompleted;
        exerciseSet.Notes = request.Notes;
        if (request.SetType.HasValue)
        {
            exerciseSet.SetType = request.SetType.Value;
        }
        if (request.DropPercentage.HasValue)
        {
            exerciseSet.DropPercentage = request.DropPercentage;
        }

        await assignedProgramRepository.UpdateAsync(assignedProgram);
        await unitOfWork.SaveChangesAsync();

        return MapWorkoutExerciseToResponse(workoutExercise);
    }

    public async Task<WorkoutExerciseResponse?> CompleteSetAsync(Guid setId, CompleteSetRequest request)
    {
        AssignedProgram? assignedProgram = await assignedProgramRepository.GetByExerciseSetIdAsync(setId);
        if (assignedProgram is null) return null;

        var exerciseSet = assignedProgram.Weeks
            .SelectMany(w => w.WorkoutDays)
            .SelectMany(d => d.Exercises)
            .SelectMany(e => e.Sets)
            .FirstOrDefault(s => s.Id == setId);

        if (exerciseSet is null) return null;

        var workoutExercise = assignedProgram.Weeks
            .SelectMany(w => w.WorkoutDays)
            .SelectMany(d => d.Exercises)
            .FirstOrDefault(e => e.Sets.Any(s => s.Id == setId));

        if (workoutExercise is null) return null;

        exerciseSet.Reps = request.Reps ?? exerciseSet.Reps;
        exerciseSet.Weight = request.Weight ?? exerciseSet.Weight;
        exerciseSet.DurationSeconds = request.DurationSeconds ?? exerciseSet.DurationSeconds;
        exerciseSet.Distance = request.Distance ?? exerciseSet.Distance;
        exerciseSet.Difficulty = request.Difficulty ?? exerciseSet.Difficulty;
        exerciseSet.Intensity = request.Intensity ?? exerciseSet.Intensity;
        exerciseSet.Notes = request.Notes ?? exerciseSet.Notes;
        exerciseSet.IsCompleted = true;
        exerciseSet.CompletedAt = DateTime.UtcNow;

        await assignedProgramRepository.UpdateAsync(assignedProgram);
        await unitOfWork.SaveChangesAsync();

        return MapWorkoutExerciseToResponse(workoutExercise);
    }

    public async Task<WorkoutExerciseResponse?> DeleteSetAsync(Guid setId)
    {
        var assignedProgram = await assignedProgramRepository.GetByExerciseSetIdAsync(setId);
        if (assignedProgram is null) return null;

        var workoutExercise = assignedProgram.Weeks
            .SelectMany(w => w.WorkoutDays)
            .SelectMany(d => d.Exercises)
            .FirstOrDefault(e => e.Sets.Any(s => s.Id == setId));

        if (workoutExercise is null) return null;

        var exerciseSet = workoutExercise.Sets.FirstOrDefault(s => s.Id == setId);
        if (exerciseSet is null) return null;

        workoutExercise.Sets.Remove(exerciseSet);

        // Renumber remaining sets
        var remainingSets = workoutExercise.Sets.OrderBy(s => s.SetNumber).ToList();
        for (int i = 0; i < remainingSets.Count; i++)
        {
            remainingSets[i].SetNumber = i + 1;
        }

        await assignedProgramRepository.UpdateAsync(assignedProgram);
        await unitOfWork.SaveChangesAsync();

        return MapWorkoutExerciseToResponse(workoutExercise);
    }

    #endregion

    #region Superset and Drop Set Operations

    public async Task<WorkoutDayResponse?> GroupExercisesInSupersetAsync(
        Guid workoutDayId,
        GroupSupersetRequest request)
    {
        if (request.ExerciseIds.Count < 2)
        {
            return null; // Need at least 2 exercises for a superset
        }

        // Find the assigned program and workout day
        var assignedProgram = await assignedProgramRepository.GetByWorkoutDayIdAsync(workoutDayId);
        if (assignedProgram is null) return null;

        var workoutDay = assignedProgram.Weeks
            .SelectMany(w => w.WorkoutDays)
            .FirstOrDefault(d => d.Id == workoutDayId);
        if (workoutDay is null) return null;

        // Generate a new superset group ID
        var supersetGroupId = Guid.NewGuid();

        // Update all exercises to have the same superset group ID
        foreach (var exerciseId in request.ExerciseIds)
        {
            var exercise = workoutDay.Exercises.FirstOrDefault(e => e.Id == exerciseId);

            if (exercise is null) return null; // All exercises must exist and belong to the workout day

            exercise.SupersetGroupId = supersetGroupId;
            exercise.SupersetRestSeconds = request.SupersetRestSeconds;
        }

        await assignedProgramRepository.UpdateAsync(assignedProgram);
        await unitOfWork.SaveChangesAsync();

        return MapWorkoutDayToResponse(workoutDay);
    }

    public async Task<WorkoutDayResponse?> UngroupExercisesFromSupersetAsync(Guid supersetGroupId)
    {
        // Find any exercise with this superset group ID to get the assigned program
        var assignedProgram = await assignedProgramRepository.GetBySupersetGroupIdAsync(supersetGroupId);
        if (assignedProgram is null) return null;

        // Find all exercises with this superset group ID
        var exercises = assignedProgram.Weeks
            .SelectMany(w => w.WorkoutDays)
            .SelectMany(d => d.Exercises)
            .Where(e => e.SupersetGroupId == supersetGroupId)
            .ToList();

        if (!exercises.Any()) return null;

        var workoutDay = assignedProgram.Weeks
            .SelectMany(w => w.WorkoutDays)
            .FirstOrDefault(d => d.Exercises.Any(e => e.SupersetGroupId == supersetGroupId));

        if (workoutDay is null) return null;

        // Remove superset grouping
        foreach (var exercise in exercises)
        {
            exercise.SupersetGroupId = null;
            exercise.SupersetRestSeconds = null;
        }

        await assignedProgramRepository.UpdateAsync(assignedProgram);
        await unitOfWork.SaveChangesAsync();

        return MapWorkoutDayToResponse(workoutDay);
    }

    public async Task<WorkoutExerciseResponse?> CreateDropSetSequenceAsync(
        Guid workoutExerciseId,
        CreateDropSetRequest request)
    {
        var assignedProgram = await assignedProgramRepository.GetByWorkoutExerciseIdAsync(workoutExerciseId);
        if (assignedProgram is null) return null;

        var workoutExercise = assignedProgram.Weeks
            .SelectMany(w => w.WorkoutDays)
            .SelectMany(d => d.Exercises)
            .FirstOrDefault(e => e.Id == workoutExerciseId);

        if (workoutExercise is null) return null;

        // Generate the drop set sequence
        var currentWeight = request.StartingWeight;
        var currentReps = request.StartingReps;

        // Determine starting set number (append to existing sets)
        var startingSetNumber = workoutExercise.Sets.Any()
            ? workoutExercise.Sets.Max(s => s.SetNumber) + 1
            : 1;

        for (int i = 0; i <= request.NumberOfDrops; i++)
        {
            var set = new ExerciseSet
            {
                Id = Guid.NewGuid(),
                WorkoutExerciseId = workoutExerciseId,
                SetNumber = startingSetNumber + i,
                Reps = currentReps,
                Weight = currentWeight,
                SetType = i == 0 ? SetType.Normal : SetType.DropSet,
                DropPercentage = i == 0 ? null : request.DropPercentage,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };

            workoutExercise.Sets.Add(set);
            await assignedProgramRepository.AddExerciseSetAsync(set);

            // Calculate next drop (if not the last iteration)
            if (i < request.NumberOfDrops)
            {
                currentWeight = Math.Round(currentWeight * (1 - request.DropPercentage / 100), 2);
                currentReps += request.RepsAdjustment;
            }
        }

        await unitOfWork.SaveChangesAsync();

        return MapWorkoutExerciseToResponse(workoutExercise);
    }

    #endregion

    #region Mapping Helpers

    private static ProgrammeWeekResponse MapWeekToResponse(ProgrammeWeek week)
    {
        return new ProgrammeWeekResponse(
            week.Id,
            week.WeekStartDate,
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
            day.Name,
            day.Description,
            day.ScheduledDate,
            day.CompletedDate,
            day.IsCompleted,
            day.IsRestDay,
            day.Exercises.Select(e => MapWorkoutExerciseToResponse(e)));
    }

    private static WorkoutExerciseResponse MapWorkoutExerciseToResponse(WorkoutExercise exercise,
        string? exerciseNameOverride = null)
    {
        return new WorkoutExerciseResponse(
            exercise.Id,
            exercise.ExerciseDefinitionId,
            exerciseNameOverride ?? exercise.ExerciseDefinition.Name,
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
            set.SetType,
            set.DropPercentage,
            set.Notes,
            set.CompletedAt);
    }

    #endregion
}