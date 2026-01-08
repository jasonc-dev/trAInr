namespace trAInr.Domain.Events;

/// <summary>
///     Domain event raised when an athlete's training level is adjusted.
/// </summary>
public record TrainingLevelAdjusted(
    Guid AthleteId,
    string PreviousLevel,
    string NewLevel,
    string Reason
) : DomainEvent;