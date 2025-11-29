namespace trAInr.API.Models.DTOs;

public record CreateWorkoutDayRequest(
    DayOfWeek DayOfWeek,
    string Name,
    string? Description,
    DateOnly? ScheduledDate,
    bool IsRestDay);

public record UpdateWorkoutDayRequest(
    string Name,
    string? Description,
    DateOnly? ScheduledDate,
    bool IsCompleted,
    bool IsRestDay);

public record WorkoutDayResponse(
    Guid Id,
    Guid ProgrammeWeekId,
    DayOfWeek DayOfWeek,
    string Name,
    string? Description,
    DateOnly? ScheduledDate,
    DateTime? CompletedDate,
    bool IsCompleted,
    bool IsRestDay,
    IEnumerable<WorkoutExerciseResponse> Exercises);

public record WorkoutDaySummaryResponse(
    Guid Id,
    string Name,
    DayOfWeek DayOfWeek,
    bool IsCompleted,
    bool IsRestDay,
    int ExerciseCount,
    int CompletedExerciseCount);

/// <summary>
/// Request to complete a workout. CompletedAt is expected in UTC.
/// </summary>
public record CompleteWorkoutRequest(
    DateTime CompletedAt);

