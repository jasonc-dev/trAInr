namespace trAInr.API.Models.DTOs;

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
    DateOnly StartDate,
    int CompletedWeeks,
    double ProgressPercentage);

public record ProgrammeWeekResponse(
    Guid Id,
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

