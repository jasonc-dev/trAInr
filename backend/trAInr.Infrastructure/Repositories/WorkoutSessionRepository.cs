using Microsoft.EntityFrameworkCore;
using trAInr.Application.Interfaces.Repositories;
using trAInr.Domain.Aggregates;
using trAInr.Domain.Entities;
using trAInr.Infrastructure.Data;

namespace trAInr.Infrastructure.Repositories;

/// <summary>
///     Repository implementation for WorkoutSession aggregate root.
/// </summary>
public class WorkoutSessionRepository(TrainrDbContext context) : IWorkoutSessionRepository
{
    public async Task<WorkoutSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.WorkoutSessions
            .Include(ws => ws.ExerciseInstances)
                .ThenInclude(ei => ei.CompletedSets)
            .FirstOrDefaultAsync(ws => ws.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<WorkoutSession>> GetByAthleteIdAsync(Guid athleteId, CancellationToken cancellationToken = default)
    {
        return await context.WorkoutSessions
            .Where(ws => ws.AthleteId == athleteId)
            .Include(ws => ws.ExerciseInstances)
                .ThenInclude(ei => ei.CompletedSets)
            .OrderByDescending(ws => ws.ScheduledDate ?? DateOnly.FromDateTime(ws.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WorkoutSession>> GetByAthleteIdAndDateRangeAsync(
        Guid athleteId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        return await context.WorkoutSessions
            .Where(ws => ws.AthleteId == athleteId &&
                         ws.ScheduledDate.HasValue &&
                         ws.ScheduledDate >= startDate &&
                         ws.ScheduledDate <= endDate)
            .Include(ws => ws.ExerciseInstances)
                .ThenInclude(ei => ei.CompletedSets)
            .OrderBy(ws => ws.ScheduledDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<WorkoutSession?> GetByAthleteIdAndDateAsync(Guid athleteId, DateOnly date, CancellationToken cancellationToken = default)
    {
        return await context.WorkoutSessions
            .Where(ws => ws.AthleteId == athleteId &&
                         ws.ScheduledDate.HasValue &&
                         ws.ScheduledDate == date)
            .Include(ws => ws.ExerciseInstances)
                .ThenInclude(ei => ei.CompletedSets)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.WorkoutSessions
            .AnyAsync(ws => ws.Id == id, cancellationToken);
    }

    public async Task AddAsync(WorkoutSession workoutSession, CancellationToken cancellationToken = default)
    {
        await context.WorkoutSessions.AddAsync(workoutSession, cancellationToken);
    }

    public async Task UpdateAsync(WorkoutSession workoutSession, CancellationToken cancellationToken = default)
    {
        context.WorkoutSessions.Update(workoutSession);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(WorkoutSession workoutSession, CancellationToken cancellationToken = default)
    {
        context.WorkoutSessions.Remove(workoutSession);
        await Task.CompletedTask;
    }

    // Sync-related methods
    public async Task<IEnumerable<WorkoutDay>> GetModifiedSinceAsync(Guid athleteId, DateTime since, CancellationToken cancellationToken = default)
    {
        // Get workout days for the athlete's programmes that were modified since the given timestamp
        return await context.WorkoutDays
            .Include(wd => wd.Exercises)
                .ThenInclude(we => we.Sets)
            .Include(wd => wd.Exercises)
                .ThenInclude(we => we.ExerciseDefinition)
            .Where(wd => wd.ProgrammeWeek.AssignedProgram.AthleteId == athleteId &&
                         wd.CreatedAt > since)
            .OrderBy(wd => wd.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<ExerciseSet?> GetExerciseSetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.ExerciseSets
            .Include(es => es.WorkoutExercise)
            .FirstOrDefaultAsync(es => es.Id == id, cancellationToken);
    }

    public async Task UpdateExerciseSetAsync(ExerciseSet set, CancellationToken cancellationToken = default)
    {
        context.ExerciseSets.Update(set);
        await Task.CompletedTask;
    }
}

