using trAInr.Domain.Enums;

namespace trAInr.Domain.Entities;

public class Tag
{
    public Guid Id { get; private set; }
    public TagNamespace Namespace { get; private set; }
    public string Name { get; private set; } = string.Empty;

    // Navigation properties
    public ICollection<ExerciseTag> ExerciseTags { get; private set; } = new List<ExerciseTag>();

    private Tag() { } // For EF Core

    public static Tag Create(TagNamespace tagNamespace, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tag name cannot be empty", nameof(name));

        return new Tag
        {
            Id = Guid.NewGuid(),
            Namespace = tagNamespace,
            Name = name
        };
    }

    public void Update(TagNamespace tagNamespace, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tag name cannot be empty", nameof(name));

        Namespace = tagNamespace;
        Name = name;
    }
}
