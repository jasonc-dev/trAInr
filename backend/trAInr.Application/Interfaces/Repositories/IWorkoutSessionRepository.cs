using trAInr.Domain.Aggregates;

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
}

