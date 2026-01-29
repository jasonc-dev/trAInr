using Microsoft.Extensions.Logging;
using trAInr.Application.Constants;
using trAInr.Application.DTOs;
using trAInr.Application.Helpers;
using trAInr.Application.Interfaces;
using trAInr.Application.Interfaces.Services;
using trAInr.Domain.Entities;

namespace trAInr.Application.Services.Exercise;

/// <summary>
/// Decorator for ExerciseDefinitionService that adds caching capabilities.
/// Implements the decorator pattern to separate caching concerns from business logic.
/// </summary>
public class CachedExerciseDefinitionService(
    IExerciseDefinitionService innerService,
    ICacheProvider cacheProvider,
    ILogger<CachedExerciseDefinitionService> logger) : IExerciseDefinitionService
{
    #region Read Operations (Cached)

    public async Task<ExerciseResponse?> GetByIdAsync(int id)
    {
        return await cacheProvider.GetOrSetNullableAsync(
            CacheKeys.ExerciseById(id),
            () => innerService.GetByIdAsync(id));
    }

    public async Task<IEnumerable<ExerciseResponse>> GetAllAsync()
    {
        // Convert IEnumerable to List for caching
        var result = await cacheProvider.GetOrSetAsync(
            CacheKeys.AllExercises,
            async () =>
            {
                var exercises = await innerService.GetAllAsync();
                return exercises.ToList();
            });

        return result;
    }

    public async Task<IEnumerable<ExerciseSummaryResponse>> SearchAsync(
        string? query,
        ExerciseType? type,
        MuscleGroup? muscleGroup)
    {
        var cacheKey = CacheKeys.ExerciseSearch(
            query,
            type?.ToString(),
            muscleGroup?.ToString());

        var result = await cacheProvider.GetOrSetAsync(
            cacheKey,
            async () =>
            {
                var exercises = await innerService.SearchAsync(query, type, muscleGroup);
                return exercises.ToList();
            });

        return result;
    }

    public async Task<IEnumerable<ExerciseSummaryResponse>> GetByTypeAsync(ExerciseType type)
    {
        var result = await cacheProvider.GetOrSetAsync(
            CacheKeys.ExercisesByType(type.ToString()),
            async () =>
            {
                var exercises = await innerService.GetByTypeAsync(type);
                return exercises.ToList();
            });

        return result;
    }

    public async Task<IEnumerable<ExerciseSummaryResponse>> GetByMuscleGroupAsync(MuscleGroup muscleGroup)
    {
        var result = await cacheProvider.GetOrSetAsync(
            CacheKeys.ExercisesByMuscleGroup(muscleGroup.ToString()),
            async () =>
            {
                var exercises = await innerService.GetByMuscleGroupAsync(muscleGroup);
                return exercises.ToList();
            });

        return result;
    }

    #endregion

    #region Write Operations (Cache Invalidation)

    public async Task<ExerciseResponse> CreateAsync(CreateExerciseRequest request, Guid? userId = null)
    {
        var result = await innerService.CreateAsync(request, userId);

        // Invalidate all list caches since a new exercise was added
        await InvalidateAllExerciseCaches();

        logger.LogInformation("Created exercise {ExerciseId}, invalidated all exercise caches", result.Id);

        return result;
    }

    public async Task<ExerciseResponse?> UpdateAsync(int id, UpdateExerciseRequest request)
    {
        var result = await innerService.UpdateAsync(id, request);

        if (result != null)
        {
            // Invalidate specific exercise cache
            await cacheProvider.RemoveAsync(CacheKeys.ExerciseById(id));

            // Invalidate all list caches since exercise details changed
            await InvalidateAllExerciseCaches();

            logger.LogInformation("Updated exercise {ExerciseId}, invalidated caches", id);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await innerService.DeleteAsync(id);

        if (result)
        {
            // Invalidate specific exercise cache
            await cacheProvider.RemoveAsync(CacheKeys.ExerciseById(id));

            // Invalidate all list caches since an exercise was removed
            await InvalidateAllExerciseCaches();

            logger.LogInformation("Deleted exercise {ExerciseId}, invalidated caches", id);
        }

        return result;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Invalidates all exercise list caches (all, by type, by muscle group, search results).
    /// Individual exercise caches are invalidated separately.
    /// </summary>
    private async Task InvalidateAllExerciseCaches()
    {
        // Invalidate the all exercises cache
        await cacheProvider.RemoveAsync(CacheKeys.AllExercises);

        // Invalidate all type-based caches
        await cacheProvider.RemoveByPrefixAsync("exercise:type");

        // Invalidate all muscle group-based caches
        await cacheProvider.RemoveByPrefixAsync("exercise:musclegroup");

        // Invalidate all search result caches
        await cacheProvider.RemoveByPrefixAsync("exercise:search");

        logger.LogDebug("Invalidated all exercise list caches");
    }

    #endregion
}