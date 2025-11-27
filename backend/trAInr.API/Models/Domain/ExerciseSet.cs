namespace trAInr.API.Models.Domain;

/// <summary>
/// Represents a single set of an exercise with actual tracked data
/// </summary>
public class ExerciseSet
{
    public Guid Id { get; set; }
    public Guid WorkoutExerciseId { get; set; }
    public int SetNumber { get; set; }
    
    // Weight training metrics
    public int? Reps { get; set; }
    public decimal? Weight { get; set; }
    
    // Cardio metrics
    public int? DurationSeconds { get; set; }
    public decimal? Distance { get; set; }
    
    // Common metrics
    public Difficulty? Difficulty { get; set; }
    public Intensity? Intensity { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsWarmup { get; set; }
    public string? Notes { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public WorkoutExercise WorkoutExercise { get; set; } = null!;
}

/// <summary>
/// Perceived difficulty of the set (RPE-like scale)
/// </summary>
public enum Difficulty
{
    VeryEasy = 1,
    Easy = 2,
    Moderate = 3,
    Hard = 4,
    VeryHard = 5,
    Maximum = 6
}

/// <summary>
/// Intensity level for the set
/// </summary>
public enum Intensity
{
    Low = 1,
    Moderate = 2,
    High = 3,
    VeryHigh = 4,
    Maximum = 5
}

