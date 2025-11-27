namespace trAInr.API.Models.DTOs;

public record CreateWorkoutDayRequest(
    DayOfWeek DayOfWeek,
    string Name,
    string? Description,
    DateTime? ScheduledDate,
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
    DayOfWeek DayOfWeek,
    string Name,
    string? Description,
    DateTime? ScheduledDate,
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

public record CompleteWorkoutRequest(
    DateTime CompletedDate);

