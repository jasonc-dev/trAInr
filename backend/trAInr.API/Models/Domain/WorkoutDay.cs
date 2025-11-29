namespace trAInr.API.Models.Domain;

/// <summary>
/// Represents a workout day within a week
/// </summary>
public class WorkoutDay
{
    public Guid Id { get; set; }
    public Guid ProgrammeWeekId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    /// <summary>
    /// Scheduled date for this workout (date only, no time component)
    /// </summary>
    public DateOnly? ScheduledDate { get; set; }
    /// <summary>
    /// When the workout was completed (UTC timestamp)
    /// </summary>
    public DateTime? CompletedDate { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsRestDay { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ProgrammeWeek ProgrammeWeek { get; set; } = null!;
    public ICollection<WorkoutExercise> Exercises { get; set; } = new List<WorkoutExercise>();
}

