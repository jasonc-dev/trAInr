using trAInr.Domain.Entities;
using trAInr.Domain.Enums;
using trAInr.Domain.ValueObjects;

namespace trAInr.Domain.Aggregates;

/// <summary>
///     ExerciseDefinition aggregate root.
///     Represents a canonical exercise definition in the exercise catalog.
/// </summary>
public class ExerciseDefinition
{
    private readonly List<EquipmentRequirement> _equipmentRequirements = new();

    // Private constructor for EF Core
    private ExerciseDefinition()
    {
    }

    public ExerciseDefinition(
        int id,
        string name,
        string description,
        ExerciseType type,
        MovementPattern movementPattern,
        MuscleGroup primaryMuscleGroup,
        MuscleGroup? secondaryMuscleGroup = null,
        LevelOfDifficulty levelOfDifficulty = LevelOfDifficulty.Beginner,
        string? instructions = null,
        string? videoUrl = null,
        bool isSystemExercise = true,
        Guid? createdByUserId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Exercise name cannot be empty", nameof(name));

        Id = id;
        Name = name;
        Description = description;
        Type = type;
        MovementPattern = movementPattern;
        PrimaryMuscleGroup = primaryMuscleGroup;
        SecondaryMuscleGroup = secondaryMuscleGroup;
        LevelOfDifficulty = levelOfDifficulty;
        Instructions = instructions;
        VideoUrl = videoUrl;
        IsSystemExercise = isSystemExercise;
        CreatedByUserId = createdByUserId;
        CreatedAt = DateTime.UtcNow;
    }

    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public ExerciseType Type { get; private set; }
    public MovementPattern MovementPattern { get; private set; }
    public MuscleGroup PrimaryMuscleGroup { get; private set; }
    public MuscleGroup? SecondaryMuscleGroup { get; private set; }
    public LevelOfDifficulty LevelOfDifficulty { get; private set; }
    public string? Instructions { get; private set; }
    public string? VideoUrl { get; private set; }
    public IReadOnlyCollection<EquipmentRequirement> EquipmentRequirements => _equipmentRequirements.AsReadOnly();
    public bool IsSystemExercise { get; private set; } = true;
    public Guid? CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    // Safety & planning fields
    public SpinalLoad SpinalLoad { get; private set; } = SpinalLoad.Low;
    public SetupComplexity SetupComplexity { get; private set; } = SetupComplexity.Low;
    public bool RequiresOverheadPosition { get; private set; }
    public bool IsUnilateral { get; private set; }
    public bool IsBodyweight { get; private set; }

    // Execution fields
    public LoadType LoadType { get; private set; } = LoadType.ExternalLoad;
    public TrackingMode TrackingMode { get; private set; } = TrackingMode.RepsWeight;
    public int? DefaultRepMin { get; private set; }
    public int? DefaultRepMax { get; private set; }
    public int? DefaultRestMinSec { get; private set; }
    public int? DefaultRestMaxSec { get; private set; }
    public int? TimePerSetEstimateSec { get; private set; }

    // Search fields
    public string[] Aliases { get; private set; } = Array.Empty<string>();
    public string? ShortCue { get; private set; }

    // Navigation properties for new normalized relationships
    public ICollection<ExerciseEquipment> ExerciseEquipments { get; private set; } = new List<ExerciseEquipment>();
    public ICollection<ExerciseMuscle> ExerciseMuscles { get; private set; } = new List<ExerciseMuscle>();
    public ICollection<ExerciseTag> ExerciseTags { get; private set; } = new List<ExerciseTag>();
    public ICollection<ExerciseVariant> ExerciseVariants { get; private set; } = new List<ExerciseVariant>();
    public ICollection<ExerciseVariant> VariantOfExercises { get; private set; } = new List<ExerciseVariant>();

    /// <summary>
    ///     Validates that the exercise can be used in a workout session.
    ///     Checks equipment requirements and constraints.
    /// </summary>
    public bool ValidateUsage(IEnumerable<EquipmentRequirement>? availableEquipment)
    {
        if (availableEquipment == null)
            return !EquipmentRequirements.Any(e => e.IsRequired);

        var availableEquipmentNames = availableEquipment.Select(e => e.Name).ToHashSet();

        return EquipmentRequirements
            .Where(e => e.IsRequired)
            .All(e => availableEquipmentNames.Contains(e.Name));
    }

    /// <summary>
    ///     Adds an equipment requirement to the exercise.
    /// </summary>
    public void AddEquipmentRequirement(EquipmentRequirement equipment)
    {
        if (_equipmentRequirements.All(e => e.Name != equipment.Name)) _equipmentRequirements.Add(equipment);
    }

    /// <summary>
    ///     Updates the exercise definition details.
    /// </summary>
    public void Update(
        string name,
        string description,
        string? instructions = null,
        string? videoUrl = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Exercise name cannot be empty", nameof(name));
        if (IsSystemExercise)
            throw new InvalidOperationException("System exercises cannot be updated");

        Name = name;
        Description = description;
        Instructions = instructions;
        VideoUrl = videoUrl;
    }
}

