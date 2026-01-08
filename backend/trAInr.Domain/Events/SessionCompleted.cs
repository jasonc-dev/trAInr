namespace trAInr.Domain.Events;

/// <summary>
///     Domain event raised when a workout session is completed.
/// </summary>
public record SessionCompleted(
    Guid WorkoutSessionId,
    Guid AthleteId,
    DateOnly SessionDate,
    int TotalSets,
    decimal TotalVolume
) : DomainEvent;