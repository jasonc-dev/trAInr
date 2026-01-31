using trAInr.Domain.Aggregates;

namespace trAInr.Domain.Entities;

public class ExerciseTag
{
    public int ExerciseDefinitionId { get; private set; }
    public Guid TagId { get; private set; }

    // Navigation properties
    public ExerciseDefinition ExerciseDefinition { get; private set; } = null!;
    public Tag Tag { get; private set; } = null!;

    private ExerciseTag() { } // For EF Core

    public static ExerciseTag Create(int exerciseDefinitionId, Guid tagId)
    {
        return new ExerciseTag
        {
            ExerciseDefinitionId = exerciseDefinitionId,
            TagId = tagId
        };
    }
}
