using trAInr.Domain.Aggregates;

namespace trAInr.Domain.Entities;

public class ExerciseEquipment
{
    public int ExerciseDefinitionId { get; private set; }
    public Guid EquipmentId { get; private set; }

    // Navigation properties
    public ExerciseDefinition ExerciseDefinition { get; private set; } = null!;
    public Equipment Equipment { get; private set; } = null!;

    private ExerciseEquipment() { } // For EF Core

    public static ExerciseEquipment Create(int exerciseDefinitionId, Guid equipmentId)
    {
        return new ExerciseEquipment
        {
            ExerciseDefinitionId = exerciseDefinitionId,
            EquipmentId = equipmentId
        };
    }
}
