using trAInr.Domain.Aggregates;
using trAInr.Domain.Enums;

namespace trAInr.Domain.Entities;

public class ExerciseMuscle
{
    public int ExerciseDefinitionId { get; private set; }
    public Guid MuscleId { get; private set; }
    public MuscleRole Role { get; private set; }
    public int? Contribution { get; private set; }

    // Navigation properties
    public ExerciseDefinition ExerciseDefinition { get; private set; } = null!;
    public Muscle Muscle { get; private set; } = null!;

    private ExerciseMuscle() { } // For EF Core

    public static ExerciseMuscle Create(int exerciseDefinitionId, Guid muscleId, MuscleRole role, int? contribution = null)
    {
        if (contribution.HasValue && (contribution.Value < 1 || contribution.Value > 100))
            throw new ArgumentException("Contribution must be between 1 and 100", nameof(contribution));

        return new ExerciseMuscle
        {
            ExerciseDefinitionId = exerciseDefinitionId,
            MuscleId = muscleId,
            Role = role,
            Contribution = contribution
        };
    }
}
