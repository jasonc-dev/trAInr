namespace trAInr.Application.DTOs.ProgramTemplate;

/// <summary>
///     Response DTO for a program template week
/// </summary>
public record ProgramTemplateWeekResponse(
    Guid Id,
    Guid ProgramTemplateId,
    int WeekNumber,
    string? Notes,
    DateTime CreatedAt,
    IEnumerable<ProgramTemplateWorkoutDayResponse> WorkoutDays);