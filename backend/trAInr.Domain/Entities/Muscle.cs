namespace trAInr.Domain.Entities;

public class Muscle
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Group { get; private set; } = string.Empty;

    // Navigation properties
    public ICollection<ExerciseMuscle> ExerciseMuscles { get; private set; } = new List<ExerciseMuscle>();

    private Muscle() { } // For EF Core

    public static Muscle Create(string name, string group)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Muscle name cannot be empty", nameof(name));

        if (string.IsNullOrWhiteSpace(group))
            throw new ArgumentException("Muscle group cannot be empty", nameof(group));

        return new Muscle
        {
            Id = Guid.NewGuid(),
            Name = name,
            Group = group
        };
    }

    public void Update(string name, string group)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Muscle name cannot be empty", nameof(name));

        if (string.IsNullOrWhiteSpace(group))
            throw new ArgumentException("Muscle group cannot be empty", nameof(group));

        Name = name;
        Group = group;
    }
}
