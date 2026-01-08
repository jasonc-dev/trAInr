namespace trAInr.Domain.Events;

/// <summary>
///     Domain event raised when a program is assigned to an athlete.
/// </summary>
public record ProgramAssigned(
    Guid AssignedProgramId,
    Guid AthleteId,
    Guid ProgramTemplateId,
    DateOnly StartDate
) : DomainEvent;