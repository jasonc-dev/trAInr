namespace trAInr.Application.DTOs;

public record CreateWorkoutDayRequest(
    DateTime? ScheduledDate,
    string Name,
    string? Description,
    bool IsRestDay);

public record UpdateWorkoutDayRequest(
    string Name,
    string? Description,
    DateTime? ScheduledDate,
    bool IsCompleted,
    bool IsRestDay);

public record WorkoutDayResponse(
    Guid Id,
    Guid ProgrammeWeekId,
    string Name,
    string? Description,
    DateOnly? ScheduledDate,
    DateTime? CompletedDate,
    bool IsCompleted,
    bool IsRestDay,
    IEnumerable<WorkoutExerciseResponse> Exercises);

/// <summary>
///     Request to complete a workout. CompletedAt is expected in UTC.
/// </summary>
public record CompleteWorkoutRequest(
    DateTime CompletedAt);
