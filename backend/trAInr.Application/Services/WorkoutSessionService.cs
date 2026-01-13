using trAInr.Application.DTOs;
using trAInr.Application.Interfaces;
using trAInr.Application.Interfaces.Repositories;
using trAInr.Application.Interfaces.Services;
using trAInr.Domain.Aggregates;
using trAInr.Domain.Entities;

namespace trAInr.Application.Services;

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

    public async Task<WorkoutDayResponse?> CreateWorkoutDayAsync(Guid weekId, CreateWorkoutDayRequest request)
    {
        var assignedProgram = await assignedProgramRepository.GetByWeekIdAsync(weekId);
        if (assignedProgram is null) return null;

        var week = assignedProgram.GetWeekById(weekId);
        if (week is null) return null;

        var workoutDay = new WorkoutDay
        {
            DayOfWeek = request.DayOfWeek,
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

        return MapWorkoutDayToResponse(addedWorkoutDay);
    }

    public async Task<WorkoutDayResponse?> UpdateWorkoutDayAsync(Guid workoutDayId, UpdateWorkoutDayRequest request)
    {
        var assignedProgram = await assignedProgramRepository.GetByWorkoutDayIdAsync(workoutDayId);
        if (assignedProgram is null) return null;

        var workoutDay = assignedProgram.Weeks
            .SelectMany(w => w.WorkoutDays)
            .FirstOrDefault(d => d.Id == workoutDayId);

        if (workoutDay is null) return null;
        
        DateOnly? ScheduledDate = workoutDay.ScheduledDate?.AddDays(request.DayOfWeek.HasValue
            ? (request.DayOfWeek.Value - workoutDay.DayOfWeek)
            : 0);

        workoutDay.Name = request.Name;
        workoutDay.Description = request.Description;
        workoutDay.DayOfWeek = request.DayOfWeek ?? workoutDay.DayOfWeek;
        workoutDay.ScheduledDate = ScheduledDate;
        workoutDay.IsCompleted = request.IsCompleted;
        workoutDay.IsRestDay = request.IsRestDay;

        var week = assignedProgram.Weeks.FirstOrDefault(w => w.WorkoutDays.Any(d => d.Id == workoutDayId));
        if (week is null) return null;

        var updatedWeek = assignedProgram.UpdateWeek(week.Id, request.IsCompleted);

        await assignedProgramRepository.UpdateAsync(assignedProgram);
        await unitOfWork.SaveChangesAsync();

        return MapWorkoutDayToResponse(workoutDay);
    }

    public async Task<bool> DeleteWorkoutDayAsync(Guid workoutDayId)
    {
        var assignedProgram = await assignedProgramRepository.GetByWorkoutDayIdAsync(workoutDayId);
        if (assignedProgram is null) return false;

        var week = assignedProgram.Weeks
            .FirstOrDefault(w => w.WorkoutDays.Any(d => d.Id == workoutDayId));

        if (week is null) return false;

        var workoutDay = week.WorkoutDays.FirstOrDefault(d => d.Id == workoutDayId);
        if (workoutDay is null) return false;

        week.WorkoutDays.Remove(workoutDay);
        await assignedProgramRepository.UpdateAsync(assignedProgram);
        await unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<WorkoutDayResponse?> CompleteWorkoutAsync(Guid workoutDayId, CompleteWorkoutRequest request)
    {
        var assignedProgram = await assignedProgramRepository.GetByWorkoutDayIdAsync(workoutDayId);
        if (assignedProgram is null) return null;

        var workoutDay = assignedProgram.Weeks
            .SelectMany(w => w.WorkoutDays)
            .FirstOrDefault(d => d.Id == workoutDayId);

        if (workoutDay is null) return null;

        workoutDay.Complete(request.CompletedAt);

        await assignedProgramRepository.UpdateAsync(assignedProgram);
        await unitOfWork.SaveChangesAsync();

        return MapWorkoutDayToResponse(workoutDay);
    }

    #endregion

    #region Workout Exercises

    public async Task<WorkoutExerciseResponse?> AddExerciseToWorkoutAsync(Guid workoutDayId,
        AddWorkoutExerciseRequest request)
    {
        var assignedProgram = await assignedProgramRepository.GetByWorkoutDayIdAsync(workoutDayId);
        if (assignedProgram is null) return null;

        var workoutDay = assignedProgram.Weeks
            .SelectMany(w => w.WorkoutDays)
            .FirstOrDefault(d => d.Id == workoutDayId);

        if (workoutDay is null) return null;

        // Verify exercise exists
        var exerciseDefinition = await exerciseDefinitionRepository.GetByIdAsync(request.ExerciseId);
        if (exerciseDefinition is null) return null;

        var workoutExercise = workoutDay.AddExercise(
            request.ExerciseId,
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

        return MapWorkoutExerciseToResponse(workoutExercise, exerciseDefinition.Name);
    }

    public async Task<WorkoutExerciseResponse?> UpdateWorkoutExerciseAsync(Guid workoutExerciseId,
        UpdateWorkoutExerciseRequest request)
    {
        var assignedProgram = await assignedProgramRepository.GetByWorkoutExerciseIdAsync(workoutExerciseId);
        if (assignedProgram is null) return null;

        var workoutExercise = assignedProgram.Weeks
            .SelectMany(w => w.WorkoutDays)
            .SelectMany(d => d.Exercises)
            .FirstOrDefault(e => e.Id == workoutExerciseId);

        if (workoutExercise is null) return null;

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

        return MapWorkoutExerciseToResponse(workoutExercise);
    }

    public async Task<bool> RemoveExerciseFromWorkoutAsync(Guid workoutExerciseId)
    {
        var assignedProgram = await assignedProgramRepository.GetByWorkoutExerciseIdAsync(workoutExerciseId);
        if (assignedProgram is null) return false;

        var workoutDay = assignedProgram.Weeks
            .SelectMany(w => w.WorkoutDays)
            .FirstOrDefault(d => d.Exercises.Any(e => e.Id == workoutExerciseId));

        if (workoutDay is null) return false;

        var removed = workoutDay.RemoveExercise(workoutExerciseId);
        if (!removed) return false;

        await assignedProgramRepository.UpdateAsync(assignedProgram);
        await unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ReorderExercisesAsync(Guid workoutDayId, List<Guid> exerciseIds)
    {
        var assignedProgram = await assignedProgramRepository.GetByWorkoutDayIdAsync(workoutDayId);
        if (assignedProgram is null) return false;

        var workoutDay = assignedProgram.Weeks
            .SelectMany(w => w.WorkoutDays)
            .FirstOrDefault(d => d.Id == workoutDayId);

        if (workoutDay is null) return false;

        var reordered = workoutDay.ReorderExercises(exerciseIds);
        if (!reordered) return false;

        await assignedProgramRepository.UpdateAsync(assignedProgram);
        await unitOfWork.SaveChangesAsync();

        return true;
    }

    #endregion

    #region Exercise Sets

    public async Task<ExerciseSetResponse?> AddSetAsync(Guid workoutExerciseId, CreateExerciseSetRequest request)
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

        return MapExerciseSetToResponse(exerciseSet);
    }

    public async Task<ExerciseSetResponse?> UpdateSetAsync(Guid setId, UpdateExerciseSetRequest request)
    {
        var assignedProgram = await assignedProgramRepository.GetByExerciseSetIdAsync(setId);
        if (assignedProgram is null) return null;

        var exerciseSet = assignedProgram.Weeks
            .SelectMany(w => w.WorkoutDays)
            .SelectMany(d => d.Exercises)
            .SelectMany(e => e.Sets)
            .FirstOrDefault(s => s.Id == setId);

        if (exerciseSet is null) return null;

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

        return MapExerciseSetToResponse(exerciseSet);
    }

    public async Task<ExerciseSetResponse?> CompleteSetAsync(Guid setId, CompleteSetRequest request)
    {
        AssignedProgram? assignedProgram = await assignedProgramRepository.GetByExerciseSetIdAsync(setId);
        if (assignedProgram is null) return null;

        var exerciseSet = assignedProgram.Weeks
            .SelectMany(w => w.WorkoutDays)
            .SelectMany(d => d.Exercises)
            .SelectMany(e => e.Sets)
            .FirstOrDefault(s => s.Id == setId);

        if (exerciseSet is null) return null;

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

        return MapExerciseSetToResponse(exerciseSet);
    }

    public async Task<bool> DeleteSetAsync(Guid setId)
    {
        var assignedProgram = await assignedProgramRepository.GetByExerciseSetIdAsync(setId);
        if (assignedProgram is null) return false;

        var workoutExercise = assignedProgram.Weeks
            .SelectMany(w => w.WorkoutDays)
            .SelectMany(d => d.Exercises)
            .FirstOrDefault(e => e.Sets.Any(s => s.Id == setId));

        if (workoutExercise is null) return false;

        var exerciseSet = workoutExercise.Sets.FirstOrDefault(s => s.Id == setId);
        if (exerciseSet is null) return false;

        workoutExercise.Sets.Remove(exerciseSet);

        // Renumber remaining sets
        var remainingSets = workoutExercise.Sets.OrderBy(s => s.SetNumber).ToList();
        for (int i = 0; i < remainingSets.Count; i++)
        {
            remainingSets[i].SetNumber = i + 1;
        }

        await assignedProgramRepository.UpdateAsync(assignedProgram);
        await unitOfWork.SaveChangesAsync();

        return true;
    }

    #endregion

    #region Superset and Drop Set Operations

    public async Task<IEnumerable<WorkoutExerciseResponse>?> GroupExercisesInSupersetAsync(
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

        var exercises = new List<WorkoutExercise>();

        // Update all exercises to have the same superset group ID
        foreach (var exerciseId in request.ExerciseIds)
        {
            var exercise = workoutDay.Exercises.FirstOrDefault(e => e.Id == exerciseId);

            if (exercise is null) return null; // All exercises must exist and belong to the workout day

            exercise.SupersetGroupId = supersetGroupId;
            exercise.SupersetRestSeconds = request.SupersetRestSeconds;
            exercises.Add(exercise);
        }

        await assignedProgramRepository.UpdateAsync(assignedProgram);
        await unitOfWork.SaveChangesAsync();

        return exercises.Select(e => MapWorkoutExerciseToResponse(e));
    }

    public async Task<bool> UngroupExercisesFromSupersetAsync(Guid supersetGroupId)
    {
        // Find any exercise with this superset group ID to get the assigned program
        var assignedProgram = await assignedProgramRepository.GetBySupersetGroupIdAsync(supersetGroupId);
        if (assignedProgram is null) return false;

        // Find all exercises with this superset group ID
        var exercises = assignedProgram.Weeks
            .SelectMany(w => w.WorkoutDays)
            .SelectMany(d => d.Exercises)
            .Where(e => e.SupersetGroupId == supersetGroupId)
            .ToList();

        if (!exercises.Any()) return false;

        // Remove superset grouping
        foreach (var exercise in exercises)
        {
            exercise.SupersetGroupId = null;
            exercise.SupersetRestSeconds = null;
        }

        await assignedProgramRepository.UpdateAsync(assignedProgram);
        await unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<ExerciseSetResponse>?> CreateDropSetSequenceAsync(
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
        var sets = new List<ExerciseSet>();
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
                Reps = (int)currentReps,
                Weight = currentWeight,
                SetType = i == 0 ? SetType.Normal : SetType.DropSet,
                DropPercentage = i == 0 ? null : request.DropPercentage,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };

            workoutExercise.Sets.Add(set);
            sets.Add(set);
            await assignedProgramRepository.AddExerciseSetAsync(set);

            // Calculate next drop (if not the last iteration)
            if (i < request.NumberOfDrops)
            {
                currentWeight = Math.Round(currentWeight * (1 - request.DropPercentage / 100), 2);
                currentReps += request.RepsAdjustment;
            }
        }

        await unitOfWork.SaveChangesAsync();

        return sets.Select(MapExerciseSetToResponse);
    }

    #endregion

    #region Mapping Helpers

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
            day.Exercises.Select(e => MapWorkoutExerciseToResponse(e)));
    }

    private static WorkoutExerciseResponse MapWorkoutExerciseToResponse(WorkoutExercise exercise,
        string? exerciseNameOverride = null)
    {
        return new WorkoutExerciseResponse(
            exercise.Id,
            exercise.ExerciseDefinitionId,
            exerciseNameOverride ?? exercise.ExerciseDefinition?.Name ?? "Unknown Exercise",
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