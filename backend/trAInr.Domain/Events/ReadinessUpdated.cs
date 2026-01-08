namespace trAInr.Domain.Events;

/// <summary>
///     Domain event raised when an athlete's readiness is updated.
/// </summary>
public record ReadinessUpdated(
    Guid AthleteId,
    decimal ReadinessScore,
    string? Notes
) : DomainEvent;