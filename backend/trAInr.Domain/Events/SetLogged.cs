namespace trAInr.Domain.Events;

/// <summary>
///     Domain event raised when a set is logged/completed.
/// </summary>
public record SetLogged(
    Guid CompletedSetId,
    Guid WorkoutSessionId,
    Guid ExerciseInstanceId,
    int Reps,
    decimal? Weight,
    int? RPE
) : DomainEvent;