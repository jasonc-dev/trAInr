namespace trAInr.Application.DTOs.ProgramTemplate;

/// <summary>
///     Response DTO for a program template workout day
/// </summary>
public record ProgramTemplateWorkoutDayResponse(
    Guid Id,
    Guid ProgramTemplateWeekId,
    string Name,
    string? Description,
    bool IsRestDay,
    DateTime CreatedAt,
    IEnumerable<ProgramTemplateWorkoutExerciseResponse> Exercises);