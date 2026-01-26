namespace trAInr.Domain.Entities;

/// <summary>
///     Represents an exercise instance within a workout day
///     Links an exercise template to a specific workout with planned sets
/// </summary>
public class WorkoutExercise
{
    public Guid Id { get; set; }
    public Guid WorkoutDayId { get; set; }
    public int ExerciseDefinitionId { get; set; }
    public int OrderIndex { get; set; }
    public string? Notes { get; set; }
    public int TargetSets { get; set; }
    public int TargetReps { get; set; }
    public decimal? TargetWeight { get; set; }
    public int? TargetDurationSeconds { get; set; }
    public decimal? TargetDistance { get; set; }

    /// <summary>
    ///     Rest time in seconds after completing this exercise (or after each exercise in a superset circuit)
    /// </summary>
    public int? RestSeconds { get; set; }

    /// <summary>
    ///     For exercises grouped in a superset/tri-set/giant set - they share the same SupersetGroupId
    ///     Null indicates this exercise is not part of a superset
    /// </summary>
    public Guid? SupersetGroupId { get; set; }

    /// <summary>
    ///     Rest time in seconds after completing the entire superset circuit (all exercises in the group)
    ///     Only applicable when SupersetGroupId is set
    /// </summary>
    public int? SupersetRestSeconds { get; set; }

    public int? TargetRpe { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public WorkoutDay WorkoutDay { get; set; } = null!;
    public Aggregates.ExerciseDefinition ExerciseDefinition { get; set; } = null!;
    public ICollection<ExerciseSet> Sets { get; set; } = new List<ExerciseSet>();
}
