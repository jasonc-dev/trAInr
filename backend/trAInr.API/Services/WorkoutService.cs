using Microsoft.EntityFrameworkCore;
using trAInr.API.Data;
using trAInr.API.Models.Domain;
using trAInr.API.Models.DTOs;

namespace trAInr.API.Services;

public class WorkoutService : IWorkoutService
{
    private readonly TrainrDbContext _context;

    public WorkoutService(TrainrDbContext context)
    {
        _context = context;
    }

    public async Task<WorkoutDayResponse?> GetWorkoutDayAsync(Guid workoutDayId)
    {
        var workoutDay = await _context.WorkoutDays
            .Include(w => w.Exercises)
                .ThenInclude(e => e.Exercise)
            .Include(w => w.Exercises)
                .ThenInclude(e => e.Sets)
            .FirstOrDefaultAsync(w => w.Id == workoutDayId);

        return workoutDay is null ? null : MapToResponse(workoutDay);
    }

    public async Task<WorkoutDayResponse?> CreateWorkoutDayAsync(Guid weekId, CreateWorkoutDayRequest request)
    {
        var week = await _context.ProgrammeWeeks.FindAsync(weekId);
        if (week is null) return null;

        var workoutDay = new WorkoutDay
        {
            Id = Guid.NewGuid(),
            ProgrammeWeekId = weekId,
            DayOfWeek = request.DayOfWeek,
            Name = request.Name,
            Description = request.Description,
            ScheduledDate = request.ScheduledDate,
            IsRestDay = request.IsRestDay,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.WorkoutDays.Add(workoutDay);
        await _context.SaveChangesAsync();

        return await GetWorkoutDayAsync(workoutDay.Id);
    }

    public async Task<WorkoutDayResponse?> UpdateWorkoutDayAsync(Guid workoutDayId, UpdateWorkoutDayRequest request)
    {
        var workoutDay = await _context.WorkoutDays.FindAsync(workoutDayId);
        if (workoutDay is null) return null;

        workoutDay.Name = request.Name;
        workoutDay.Description = request.Description;
        workoutDay.ScheduledDate = request.ScheduledDate;
        workoutDay.IsCompleted = request.IsCompleted;
        workoutDay.IsRestDay = request.IsRestDay;

        await _context.SaveChangesAsync();
        return await GetWorkoutDayAsync(workoutDayId);
    }

    public async Task<bool> DeleteWorkoutDayAsync(Guid workoutDayId)
    {
        var workoutDay = await _context.WorkoutDays.FindAsync(workoutDayId);
        if (workoutDay is null) return false;

        _context.WorkoutDays.Remove(workoutDay);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<WorkoutDayResponse?> CompleteWorkoutAsync(Guid workoutDayId, CompleteWorkoutRequest request)
    {
        var workoutDay = await _context.WorkoutDays
            .Include(w => w.Exercises)
                .ThenInclude(e => e.Sets)
            .FirstOrDefaultAsync(w => w.Id == workoutDayId);

        if (workoutDay is null) return null;

        workoutDay.IsCompleted = true;
        workoutDay.CompletedDate = request.CompletedDate;

        // Mark all incomplete sets as completed
        foreach (var exercise in workoutDay.Exercises)
        {
            foreach (var set in exercise.Sets.Where(s => !s.IsCompleted))
            {
                set.IsCompleted = true;
                set.CompletedAt = request.CompletedDate;
            }
        }

        await _context.SaveChangesAsync();
        return await GetWorkoutDayAsync(workoutDayId);
    }

    public async Task<WorkoutExerciseResponse?> AddExerciseToWorkoutAsync(
        Guid workoutDayId, 
        AddWorkoutExerciseRequest request)
    {
        var workoutDay = await _context.WorkoutDays.FindAsync(workoutDayId);
        var exercise = await _context.Exercises.FindAsync(request.ExerciseId);

        if (workoutDay is null || exercise is null) return null;

        var workoutExercise = new WorkoutExercise
        {
            Id = Guid.NewGuid(),
            WorkoutDayId = workoutDayId,
            ExerciseId = request.ExerciseId,
            OrderIndex = request.OrderIndex,
            Notes = request.Notes,
            TargetSets = request.TargetSets,
            TargetReps = request.TargetReps,
            TargetWeight = request.TargetWeight,
            TargetDurationSeconds = request.TargetDurationSeconds,
            TargetDistance = request.TargetDistance,
            CreatedAt = DateTime.UtcNow
        };

        _context.WorkoutExercises.Add(workoutExercise);
        await _context.SaveChangesAsync();

        return await GetWorkoutExerciseAsync(workoutExercise.Id);
    }

    public async Task<WorkoutExerciseResponse?> UpdateWorkoutExerciseAsync(
        Guid workoutExerciseId, 
        UpdateWorkoutExerciseRequest request)
    {
        var workoutExercise = await _context.WorkoutExercises.FindAsync(workoutExerciseId);
        if (workoutExercise is null) return null;

        workoutExercise.OrderIndex = request.OrderIndex;
        workoutExercise.Notes = request.Notes;
        workoutExercise.TargetSets = request.TargetSets;
        workoutExercise.TargetReps = request.TargetReps;
        workoutExercise.TargetWeight = request.TargetWeight;
        workoutExercise.TargetDurationSeconds = request.TargetDurationSeconds;
        workoutExercise.TargetDistance = request.TargetDistance;

        await _context.SaveChangesAsync();
        return await GetWorkoutExerciseAsync(workoutExerciseId);
    }

    public async Task<bool> RemoveExerciseFromWorkoutAsync(Guid workoutExerciseId)
    {
        var workoutExercise = await _context.WorkoutExercises.FindAsync(workoutExerciseId);
        if (workoutExercise is null) return false;

        _context.WorkoutExercises.Remove(workoutExercise);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReorderExercisesAsync(Guid workoutDayId, List<Guid> exerciseIds)
    {
        var exercises = await _context.WorkoutExercises
            .Where(e => e.WorkoutDayId == workoutDayId)
            .ToListAsync();

        for (int i = 0; i < exerciseIds.Count; i++)
        {
            var exercise = exercises.FirstOrDefault(e => e.Id == exerciseIds[i]);
            if (exercise is not null)
            {
                exercise.OrderIndex = i;
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ExerciseSetResponse?> AddSetAsync(Guid workoutExerciseId, CreateExerciseSetRequest request)
    {
        var workoutExercise = await _context.WorkoutExercises.FindAsync(workoutExerciseId);
        if (workoutExercise is null) return null;

        var set = new ExerciseSet
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
            IsWarmup = request.IsWarmup,
            Notes = request.Notes,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.ExerciseSets.Add(set);
        await _context.SaveChangesAsync();

        return MapToSetResponse(set);
    }

    public async Task<ExerciseSetResponse?> UpdateSetAsync(Guid setId, UpdateExerciseSetRequest request)
    {
        var set = await _context.ExerciseSets.FindAsync(setId);
        if (set is null) return null;

        set.Reps = request.Reps;
        set.Weight = request.Weight;
        set.DurationSeconds = request.DurationSeconds;
        set.Distance = request.Distance;
        set.Difficulty = request.Difficulty;
        set.Intensity = request.Intensity;
        set.IsCompleted = request.IsCompleted;
        set.Notes = request.Notes;

        if (request.IsCompleted && set.CompletedAt is null)
        {
            set.CompletedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return MapToSetResponse(set);
    }

    public async Task<ExerciseSetResponse?> CompleteSetAsync(Guid setId, CompleteSetRequest request)
    {
        var set = await _context.ExerciseSets.FindAsync(setId);
        if (set is null) return null;

        set.Reps = request.Reps ?? set.Reps;
        set.Weight = request.Weight ?? set.Weight;
        set.DurationSeconds = request.DurationSeconds ?? set.DurationSeconds;
        set.Distance = request.Distance ?? set.Distance;
        set.Difficulty = request.Difficulty ?? set.Difficulty;
        set.Intensity = request.Intensity ?? set.Intensity;
        set.Notes = request.Notes ?? set.Notes;
        set.IsCompleted = true;
        set.CompletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapToSetResponse(set);
    }

    public async Task<bool> DeleteSetAsync(Guid setId)
    {
        var set = await _context.ExerciseSets.FindAsync(setId);
        if (set is null) return false;

        _context.ExerciseSets.Remove(set);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<WorkoutExerciseResponse?> GetWorkoutExerciseAsync(Guid id)
    {
        var workoutExercise = await _context.WorkoutExercises
            .Include(e => e.Exercise)
            .Include(e => e.Sets)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (workoutExercise is null) return null;

        return new WorkoutExerciseResponse(
            workoutExercise.Id,
            workoutExercise.ExerciseId,
            workoutExercise.Exercise?.Name ?? "Unknown",
            workoutExercise.OrderIndex,
            workoutExercise.Notes,
            workoutExercise.TargetSets,
            workoutExercise.TargetReps,
            workoutExercise.TargetWeight,
            workoutExercise.TargetDurationSeconds,
            workoutExercise.TargetDistance,
            workoutExercise.Sets.OrderBy(s => s.SetNumber).Select(MapToSetResponse));
    }

    private static WorkoutDayResponse MapToResponse(WorkoutDay w) =>
        new(
            w.Id,
            w.ProgrammeWeekId,
            w.DayOfWeek,
            w.Name,
            w.Description,
            w.ScheduledDate,
            w.CompletedDate,
            w.IsCompleted,
            w.IsRestDay,
            w.Exercises.OrderBy(e => e.OrderIndex).Select(e => new WorkoutExerciseResponse(
                e.Id,
                e.ExerciseId,
                e.Exercise?.Name ?? "Unknown",
                e.OrderIndex,
                e.Notes,
                e.TargetSets,
                e.TargetReps,
                e.TargetWeight,
                e.TargetDurationSeconds,
                e.TargetDistance,
                e.Sets.OrderBy(s => s.SetNumber).Select(MapToSetResponse))));

    private static ExerciseSetResponse MapToSetResponse(ExerciseSet s) =>
        new(
            s.Id,
            s.SetNumber,
            s.Reps,
            s.Weight,
            s.DurationSeconds,
            s.Distance,
            s.Difficulty,
            s.Intensity,
            s.IsCompleted,
            s.IsWarmup,
            s.Notes,
            s.CompletedAt);
}

