using Microsoft.EntityFrameworkCore;
using trAInr.Application.Interfaces.Repositories;
using trAInr.Domain.Aggregates;
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
}

