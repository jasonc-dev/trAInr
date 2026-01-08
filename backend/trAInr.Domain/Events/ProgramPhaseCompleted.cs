namespace trAInr.Domain.Events;

/// <summary>
///     Domain event raised when a program phase is completed.
/// </summary>
public record ProgramPhaseCompleted(
    Guid AssignedProgramId,
    Guid AthleteId,
    int PhaseNumber,
    DateOnly CompletedDate
) : DomainEvent;