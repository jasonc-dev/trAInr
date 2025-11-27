namespace trAInr.API.Models.Domain;

/// <summary>
/// Represents an exercise instance within a workout day
/// Links an exercise template to a specific workout with planned sets
/// </summary>
public class WorkoutExercise
{
    public Guid Id { get; set; }
    public Guid WorkoutDayId { get; set; }
    public Guid ExerciseId { get; set; }
    public int OrderIndex { get; set; }
    public string? Notes { get; set; }
    public int TargetSets { get; set; }
    public int TargetReps { get; set; }
    public decimal? TargetWeight { get; set; }
    public int? TargetDurationSeconds { get; set; }
    public decimal? TargetDistance { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public WorkoutDay WorkoutDay { get; set; } = null!;
    public Exercise Exercise { get; set; } = null!;
    public ICollection<ExerciseSet> Sets { get; set; } = new List<ExerciseSet>();
}

