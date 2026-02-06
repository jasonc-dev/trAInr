using trAInr.Domain.Aggregates;
using trAInr.Domain.Entities;

namespace trAInr.Application.Interfaces.Repositories;

/// <summary>
///     Repository interface for WorkoutSession aggregate root.
/// </summary>
public interface IWorkoutSessionRepository
{
    Task<WorkoutSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkoutSession>> GetByAthleteIdAsync(Guid athleteId, CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkoutSession>> GetByAthleteIdAndDateRangeAsync(
        Guid athleteId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default);
    Task<WorkoutSession?> GetByAthleteIdAndDateAsync(Guid athleteId, DateOnly date, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(WorkoutSession workoutSession, CancellationToken cancellationToken = default);
    Task UpdateAsync(WorkoutSession workoutSession, CancellationToken cancellationToken = default);
    Task DeleteAsync(WorkoutSession workoutSession, CancellationToken cancellationToken = default);

    // Sync-related methods
    Task<IEnumerable<WorkoutDay>> GetModifiedSinceAsync(Guid athleteId, DateTime since, CancellationToken cancellationToken = default);
    Task<ExerciseSet?> GetExerciseSetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateExerciseSetAsync(ExerciseSet set, CancellationToken cancellationToken = default);
}

