using trAInr.Domain.Aggregates;

namespace trAInr.Domain.Entities;

/// <summary>
///     Represents a single week within a programme
/// </summary>
public class ProgrammeWeek
{
    public Guid Id { get; set; }
    public Guid AssignedProgramId { get; set; }
    public int WeekNumber { get; set; }
    public string? Notes { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public AssignedProgram AssignedProgram { get; set; } = null!;
    public ICollection<WorkoutDay> WorkoutDays { get; set; } = new List<WorkoutDay>();
}