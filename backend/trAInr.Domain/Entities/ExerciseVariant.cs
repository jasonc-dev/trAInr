using trAInr.Domain.Aggregates;
using trAInr.Domain.Enums;

namespace trAInr.Domain.Entities;

public class ExerciseVariant
{
    public int ExerciseDefinitionId { get; private set; }
    public int VariantOfExerciseDefinitionId { get; private set; }
    public VariantType VariantType { get; private set; }

    // Navigation properties
    public ExerciseDefinition ExerciseDefinition { get; private set; } = null!;
    public ExerciseDefinition VariantOfExerciseDefinition { get; private set; } = null!;

    private ExerciseVariant() { } // For EF Core

    public static ExerciseVariant Create(int exerciseDefinitionId, int variantOfExerciseDefinitionId, VariantType variantType)
    {
        if (exerciseDefinitionId == variantOfExerciseDefinitionId)
            throw new ArgumentException("Exercise cannot be a variant of itself");

        return new ExerciseVariant
        {
            ExerciseDefinitionId = exerciseDefinitionId,
            VariantOfExerciseDefinitionId = variantOfExerciseDefinitionId,
            VariantType = variantType
        };
    }
}
