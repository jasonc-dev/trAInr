using trAInr.Domain.Entities;
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
        Guid id,
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

    public Guid Id { get; private set; }
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

