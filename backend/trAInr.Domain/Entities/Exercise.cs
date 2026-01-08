namespace trAInr.Domain.Entities;

/// <summary>
///     Represents an exercise definition (template)
/// </summary>
public class Exercise
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public ExerciseType Type { get; set; }
  public MuscleGroup PrimaryMuscleGroup { get; set; }
  public MuscleGroup? SecondaryMuscleGroup { get; set; }
  public string? Instructions { get; set; }
  public string? VideoUrl { get; set; }
  public bool IsSystemExercise { get; set; } = true;
  public Guid? CreatedByUserId { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
///     Type of exercise - determines which metrics are tracked
/// </summary>
public enum ExerciseType
{
  WeightTraining = 1,
  Cardio = 2,
  Bodyweight = 3,
  Flexibility = 4
}

/// <summary>
///     Primary muscle groups for exercise categorization
/// </summary>
public enum MuscleGroup
{
  Chest = 1,
  Back = 2,
  Shoulders = 3,
  Biceps = 4,
  Triceps = 5,
  Forearms = 6,
  Core = 7,
  Quadriceps = 8,
  Hamstrings = 9,
  Glutes = 10,
  Calves = 11,
  FullBody = 12,
  Cardio = 13
}