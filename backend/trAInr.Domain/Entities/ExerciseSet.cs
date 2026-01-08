namespace trAInr.Domain.Entities;

/// <summary>
///     Represents a single set of an exercise with actual tracked data
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
    
    /// <summary>
    ///     Deprecated: Use SetType instead. Kept for backward compatibility.
    /// </summary>
    [Obsolete("Use SetType property instead")]
    public bool IsWarmup { get; set; }
    
    /// <summary>
    ///     Type of set (Normal, Warmup, DropSet, etc.)
    /// </summary>
    public SetType SetType { get; set; } = SetType.Normal;
    
    /// <summary>
    ///     For drop sets: the percentage reduction from the previous set's weight
    ///     e.g., 20.0 means 20% weight reduction
    /// </summary>
    public decimal? DropPercentage { get; set; }
    
    public string? Notes { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public WorkoutExercise WorkoutExercise { get; set; } = null!;
}

/// <summary>
///     Perceived difficulty of the set (RPE-like scale)
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
///     Intensity level for the set
/// </summary>
public enum Intensity
{
    Low = 1,
    Moderate = 2,
    High = 3,
    VeryHigh = 4,
    Maximum = 5
}

/// <summary>
///     Type of set being performed
/// </summary>
public enum SetType
{
    /// <summary>
    ///     Regular working set
    /// </summary>
    Normal = 0,
    
    /// <summary>
    ///     Warmup set with lighter weight
    /// </summary>
    Warmup = 1,
    
    /// <summary>
    ///     Drop set - reduced weight for additional reps
    /// </summary>
    DropSet = 2,
    
    /// <summary>
    ///     Set taken to failure
    /// </summary>
    Failure = 3,
    
    /// <summary>
    ///     As Many Reps As Possible
    /// </summary>
    Amrap = 4
}
