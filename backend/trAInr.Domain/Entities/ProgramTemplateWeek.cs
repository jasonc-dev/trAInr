using trAInr.Domain.Aggregates;

namespace trAInr.Domain.Entities;

/// <summary>
///     Represents a single week within a program template
/// </summary>
public class ProgramTemplateWeek
{
    public Guid Id { get; set; }
    public Guid ProgramTemplateId { get; set; }
    public int WeekNumber { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ProgramTemplate ProgramTemplate { get; set; } = null!;
    public ICollection<ProgramTemplateWorkoutDay> WorkoutDays { get; set; } = new List<ProgramTemplateWorkoutDay>();
}