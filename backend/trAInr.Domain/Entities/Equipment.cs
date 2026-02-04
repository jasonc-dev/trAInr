namespace trAInr.Domain.Entities;

public class Equipment
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public bool IsPortable { get; private set; }

    // Navigation properties
    public ICollection<ExerciseEquipment> ExerciseEquipments { get; private set; } = [];

    private Equipment() { } // For EF Core

    public static Equipment Create(string name, string category, bool isPortable = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Equipment name cannot be empty", nameof(name));

        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Equipment category cannot be empty", nameof(category));

        return new Equipment
        {
            Id = Guid.NewGuid(),
            Name = name,
            Category = category,
            IsPortable = isPortable
        };
    }

    public void Update(string name, string category, bool isPortable)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Equipment name cannot be empty", nameof(name));

        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Equipment category cannot be empty", nameof(category));

        Name = name;
        Category = category;
        IsPortable = isPortable;
    }
}
