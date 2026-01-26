using trAInr.Domain.Entities;

namespace trAInr.Application.DTOs.ProgramTemplate;

/// <summary>
///     Response DTO for a program template (used for AI-generated programs)
/// </summary>
public record ProgramTemplateResponse(
    Guid Id,
    string Name,
    string Description,
    int DurationWeeks,
    ExperienceLevel ExperienceLevel,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IEnumerable<ProgramTemplateWeekResponse> Weeks);