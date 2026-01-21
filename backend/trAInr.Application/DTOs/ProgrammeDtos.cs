namespace trAInr.Application.DTOs;

public record CreateProgrammeRequest(
    string Name,
    string Description,
    int DurationWeeks,
    DateOnly StartDate);

public record UpdateProgrammeRequest(
    string Name,
    string Description,
    bool IsActive);

public record ProgrammeResponse(
    Guid Id,
    Guid UserId,
    string Name,
    string Description,
    int DurationWeeks,
    bool IsPreMade,
    bool IsActive,
    DateOnly StartDate,
    DateOnly? EndDate,
    DateTime CreatedAt,
    IEnumerable<ProgrammeWeekResponse> Weeks);

public record ProgrammeSummaryResponse(
    Guid Id,
    string Name,
    string Description,
    int DurationWeeks,
    bool IsActive,
    bool IsPreMade,
    DateOnly StartDate,
    int CompletedWeeks,
    double ProgressPercentage);

public record ProgrammeWeekResponse(
    Guid Id,
    DateOnly WeekStartDate,
    int WeekNumber,
    string? Notes,
    bool IsCompleted,
    IEnumerable<WorkoutDayResponse> WorkoutDays);

public record CreateProgrammeWeekRequest(
    int WeekNumber,
    string? Notes);

public record UpdateProgrammeWeekRequest(
    string? Notes,
    bool IsCompleted);

public record CopyWeekRequest(
    int TargetWeekNumber);

public record CloneProgrammeRequest(
    Guid AthleteId,
    DateOnly StartDate);