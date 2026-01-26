using trAInr.Domain.Aggregates;
using trAInr.Domain.Entities;

namespace trAInr.Application.Interfaces.Repositories;

/// <summary>
///     Repository interface for ExerciseDefinition aggregate root.
/// </summary>
public interface IExerciseDefinitionRepository
{
    Task<ExerciseDefinition?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ExerciseDefinition>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ExerciseDefinition>> SearchAsync(
        string? query = null,
        ExerciseType? type = null,
        MuscleGroup? muscleGroup = null,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<ExerciseDefinition>> GetByTypeAsync(ExerciseType type, CancellationToken cancellationToken = default);
    Task<IEnumerable<ExerciseDefinition>> GetByMuscleGroupAsync(MuscleGroup muscleGroup, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(ExerciseDefinition exerciseDefinition, CancellationToken cancellationToken = default);
    Task UpdateAsync(ExerciseDefinition exerciseDefinition, CancellationToken cancellationToken = default);
    Task DeleteAsync(ExerciseDefinition exerciseDefinition, CancellationToken cancellationToken = default);
}

