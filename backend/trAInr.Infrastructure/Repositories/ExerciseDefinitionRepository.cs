using Microsoft.EntityFrameworkCore;
using trAInr.Application.Interfaces.Repositories;
using trAInr.Domain.Aggregates;
using trAInr.Domain.Entities;
using trAInr.Infrastructure.Data;

namespace trAInr.Infrastructure.Repositories;

/// <summary>
///     Repository implementation for ExerciseDefinition aggregate root.
/// </summary>
public class ExerciseDefinitionRepository(TrainrDbContext context) : IExerciseDefinitionRepository
{
    public async Task<ExerciseDefinition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.ExerciseDefinitions
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<ExerciseDefinition>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.ExerciseDefinitions
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ExerciseDefinition>> SearchAsync(
        string? query = null,
        ExerciseType? type = null,
        MuscleGroup? muscleGroup = null,
        CancellationToken cancellationToken = default)
    {
        var queryable = context.ExerciseDefinitions.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var lowerQuery = query.ToLower();
            queryable = queryable.Where(e =>
                e.Name.ToLower().Contains(lowerQuery) ||
                (e.Description != null && e.Description.ToLower().Contains(lowerQuery)));
        }

        if (type.HasValue)
        {
            queryable = queryable.Where(e => e.Type == type.Value);
        }

        if (muscleGroup.HasValue)
        {
            queryable = queryable.Where(e =>
                e.PrimaryMuscleGroup == muscleGroup.Value ||
                e.SecondaryMuscleGroup == muscleGroup.Value);
        }

        return await queryable
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ExerciseDefinition>> GetByTypeAsync(ExerciseType type, CancellationToken cancellationToken = default)
    {
        return await context.ExerciseDefinitions
            .Where(e => e.Type == type)
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ExerciseDefinition>> GetByMuscleGroupAsync(MuscleGroup muscleGroup, CancellationToken cancellationToken = default)
    {
        return await context.ExerciseDefinitions
            .Where(e =>
                e.PrimaryMuscleGroup == muscleGroup ||
                e.SecondaryMuscleGroup == muscleGroup)
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.ExerciseDefinitions
            .AnyAsync(e => e.Id == id, cancellationToken);
    }

    public async Task AddAsync(ExerciseDefinition exerciseDefinition, CancellationToken cancellationToken = default)
    {
        await context.ExerciseDefinitions.AddAsync(exerciseDefinition, cancellationToken);
    }

    public async Task UpdateAsync(ExerciseDefinition exerciseDefinition, CancellationToken cancellationToken = default)
    {
        context.ExerciseDefinitions.Update(exerciseDefinition);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(ExerciseDefinition exerciseDefinition, CancellationToken cancellationToken = default)
    {
        context.ExerciseDefinitions.Remove(exerciseDefinition);
        await Task.CompletedTask;
    }
}

