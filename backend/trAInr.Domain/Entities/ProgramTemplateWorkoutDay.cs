namespace trAInr.Domain.Entities;

/// <summary>
///     Represents a workout day within a program template week
/// </summary>
public class ProgramTemplateWorkoutDay
{
    public Guid Id { get; set; }
    public Guid ProgramTemplateWeekId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsRestDay { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ProgramTemplateWeek ProgramTemplateWeek { get; set; } = null!;
    public ICollection<ProgramTemplateWorkoutExercise> Exercises { get; set; } = new List<ProgramTemplateWorkoutExercise>();
}