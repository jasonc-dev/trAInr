namespace trAInr.Domain.ValueObjects;

/// <summary>
///     Value object representing equipment requirements for an exercise.
/// </summary>
public record struct EquipmentRequirement
{
    private EquipmentRequirement(string name, bool isRequired)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Equipment name cannot be empty", nameof(name));

        Name = name;
        IsRequired = isRequired;
    }

    public string Name { get; init; }
    public bool IsRequired { get; init; }

    public static EquipmentRequirement Create(string name, bool isRequired = true)
    {
        return new EquipmentRequirement(name, isRequired);
    }

    public static EquipmentRequirement Optional(string name)
    {
        return new EquipmentRequirement(name, false);
    }

    public static EquipmentRequirement Required(string name)
    {
        return new EquipmentRequirement(name, true);
    }
}