using Microsoft.Extensions.Logging;
using trAInr.Application.Constants;
using trAInr.Application.DTOs;
using trAInr.Application.Interfaces;
using trAInr.Application.Interfaces.Repositories;
using trAInr.Application.Interfaces.Services;

namespace trAInr.Application.Services.WorkoutSession;

/// <summary>
/// Decorator for WorkoutSessionService that adds caching capabilities.
/// Implements the decorator pattern to separate caching concerns from business logic.
/// Also invalidates assigned programme caches to ensure consistency across the system.
/// </summary>
public class CachedWorkoutSessionService(
    IWorkoutSessionService innerService,
    ICacheProvider cacheProvider,
    IAssignedProgramRepository assignedProgramRepository,
    ILogger<CachedWorkoutSessionService> logger) : IWorkoutSessionService
{
    #region Read Operations (Cached)

    public async Task<WorkoutDayResponse?> GetWorkoutDayAsync(Guid workoutDayId)
    {
        var cacheKey = CacheKeys.WorkoutDayById(workoutDayId);

        var cachedResult = await cacheProvider.GetAsync<WorkoutDayResponse>(cacheKey);

        if (cachedResult != null)
        {
            return cachedResult;
        }

        var result = await innerService.GetWorkoutDayAsync(workoutDayId);

        if (result != null)
        {
            await cacheProvider.SetAsync(cacheKey, result);
        }

        return result;
    }


    #endregion

    #region Write Operations (Cache Invalidation)

    public async Task<WorkoutDayResponse?> CreateWorkoutDayAsync(Guid weekId, CreateWorkoutDayRequest request)
    {
        var result = await innerService.CreateWorkoutDayAsync(weekId, request);

        if (result != null)
        {
            await InvalidateWorkoutDayCache(result.Id);
            await InvalidateProgrammeCacheByWeekId(weekId);
        }

        logger.LogInformation("Created workout day {WorkoutDayId} for week {WeekId}, invalidated caches", result?.Id, weekId);

        return result;
    }

    public async Task<WorkoutDayResponse?> UpdateWorkoutDayAsync(Guid workoutDayId, UpdateWorkoutDayRequest request)
    {
        var result = await innerService.UpdateWorkoutDayAsync(workoutDayId, request);

        if (result != null)
        {
            await InvalidateWorkoutDayCache(workoutDayId);
            await InvalidateProgrammeCacheByWorkoutDayId(workoutDayId);
        }

        logger.LogInformation("Updated workout day {WorkoutDayId}, invalidated cache", workoutDayId);

        return result;
    }

    public async Task<bool> DeleteWorkoutDayAsync(Guid workoutDayId)
    {
        // Get programme ID before deletion
        var assignedProgram = await assignedProgramRepository.GetByWorkoutDayIdAsync(workoutDayId);

        var result = await innerService.DeleteWorkoutDayAsync(workoutDayId);

        if (result && assignedProgram != null)
        {
            await InvalidateWorkoutDayCache(workoutDayId);
            await InvalidateProgrammeCaches(assignedProgram.Id, assignedProgram.AthleteId);
        }

        logger.LogInformation("Deleted workout day {WorkoutDayId}, invalidated cache", workoutDayId);

        return result;
    }

    public async Task<WorkoutDayResponse?> CompleteWorkoutAsync(Guid workoutDayId, CompleteWorkoutRequest request)
    {
        var result = await innerService.CompleteWorkoutAsync(workoutDayId, request);

        if (result != null)
        {
            await InvalidateWorkoutDayCache(workoutDayId);
            await InvalidateProgrammeCacheByWorkoutDayId(workoutDayId);
        }

        logger.LogInformation("Completed workout day {WorkoutDayId}, invalidated cache", workoutDayId);

        return result;
    }

    public async Task<WorkoutExerciseResponse?> AddExerciseToWorkoutAsync(Guid workoutDayId, AddWorkoutExerciseRequest request)
    {
        var result = await innerService.AddExerciseToWorkoutAsync(workoutDayId, request);

        if (result != null)
        {
            // Invalidate the workout day cache since it contains the exercises list
            await InvalidateWorkoutDayCache(workoutDayId);
            await InvalidateProgrammeCacheByWorkoutDayId(workoutDayId);
        }

        logger.LogInformation("Added exercise {ExerciseId} to workout day {WorkoutDayId}, invalidated caches", result?.Id, workoutDayId);

        return result;
    }

    public async Task<WorkoutExerciseResponse?> UpdateWorkoutExerciseAsync(Guid workoutExerciseId, UpdateWorkoutExerciseRequest request)
    {
        var result = await innerService.UpdateWorkoutExerciseAsync(workoutExerciseId, request);

        if (result != null)
        {
            await InvalidateWorkoutExerciseCache(workoutExerciseId);
            await InvalidateProgrammeCacheByWorkoutExerciseId(workoutExerciseId);
        }

        logger.LogInformation("Updated workout exercise {WorkoutExerciseId}, invalidated cache", workoutExerciseId);

        return result;
    }

    public async Task<bool> RemoveExerciseFromWorkoutAsync(Guid workoutExerciseId)
    {
        // Get programme ID before deletion
        var assignedProgram = await assignedProgramRepository.GetByWorkoutExerciseIdAsync(workoutExerciseId);

        var result = await innerService.RemoveExerciseFromWorkoutAsync(workoutExerciseId);

        if (result && assignedProgram != null)
        {
            await InvalidateWorkoutExerciseCache(workoutExerciseId);
            await InvalidateProgrammeCaches(assignedProgram.Id, assignedProgram.AthleteId);
        }

        logger.LogInformation("Removed workout exercise {WorkoutExerciseId}, invalidated cache", workoutExerciseId);

        return result;
    }

    public async Task<bool> ReorderExercisesAsync(Guid workoutDayId, List<Guid> workoutExerciseIds)
    {
        var result = await innerService.ReorderExercisesAsync(workoutDayId, workoutExerciseIds);

        if (result)
        {
            // Invalidate the workout day cache since exercise order changed
            await InvalidateWorkoutDayCache(workoutDayId);
            await InvalidateProgrammeCacheByWorkoutDayId(workoutDayId);
        }

        logger.LogInformation("Reordered exercises for workout day {WorkoutDayId}, invalidated caches", workoutDayId);

        return result;
    }

    public async Task<ExerciseSetResponse?> AddSetAsync(Guid workoutExerciseId, CreateExerciseSetRequest request)
    {
        var result = await innerService.AddSetAsync(workoutExerciseId, request);

        if (result != null)
        {
            // Invalidate the workout exercise cache since it contains the sets list
            await InvalidateWorkoutExerciseCache(workoutExerciseId);
            await InvalidateProgrammeCacheByWorkoutExerciseId(workoutExerciseId);
        }

        logger.LogInformation("Added set {SetId} to workout exercise {WorkoutExerciseId}, invalidated caches", result?.Id, workoutExerciseId);

        return result;
    }

    public async Task<ExerciseSetResponse?> UpdateSetAsync(Guid setId, UpdateExerciseSetRequest request)
    {
        var result = await innerService.UpdateSetAsync(setId, request);

        if (result != null)
        {
            await InvalidateExerciseSetCache(setId);
            await InvalidateProgrammeCacheByExerciseSetId(setId);
        }

        logger.LogInformation("Updated exercise set {SetId}, invalidated cache", setId);

        return result;
    }

    public async Task<ExerciseSetResponse?> CompleteSetAsync(Guid setId, CompleteSetRequest request)
    {
        var result = await innerService.CompleteSetAsync(setId, request);

        if (result != null)
        {
            await InvalidateExerciseSetCache(setId);
            await InvalidateProgrammeCacheByExerciseSetId(setId);
        }

        logger.LogInformation("Completed exercise set {SetId}, invalidated cache", setId);

        return result;
    }

    public async Task<bool> DeleteSetAsync(Guid setId)
    {
        // Get programme ID before deletion
        var assignedProgram = await assignedProgramRepository.GetByExerciseSetIdAsync(setId);

        var result = await innerService.DeleteSetAsync(setId);

        if (result && assignedProgram != null)
        {
            await InvalidateExerciseSetCache(setId);
            await InvalidateProgrammeCaches(assignedProgram.Id, assignedProgram.AthleteId);
        }

        logger.LogInformation("Deleted exercise set {SetId}, invalidated cache", setId);

        return result;
    }

    public async Task<IEnumerable<WorkoutExerciseResponse>?> GroupExercisesInSupersetAsync(Guid workoutDayId, GroupSupersetRequest request)
    {
        var result = await innerService.GroupExercisesInSupersetAsync(workoutDayId, request);

        if (result != null)
        {
            // Invalidate the workout day cache since superset grouping changed
            await InvalidateWorkoutDayCache(workoutDayId);
            await InvalidateProgrammeCacheByWorkoutDayId(workoutDayId);

            // Invalidate each individual exercise cache in the superset
            foreach (var exerciseId in request.ExerciseIds)
            {
                await InvalidateWorkoutExerciseCache(exerciseId);
            }
        }

        logger.LogInformation("Grouped exercises in superset for workout day {WorkoutDayId}, invalidated caches", workoutDayId);

        return result;
    }

    public async Task<bool> UngroupExercisesFromSupersetAsync(Guid supersetGroupId)
    {
        // Get programme ID before ungrouping
        var assignedProgram = await assignedProgramRepository.GetBySupersetGroupIdAsync(supersetGroupId);

        var result = await innerService.UngroupExercisesFromSupersetAsync(supersetGroupId);

        if (result && assignedProgram != null)
        {
            await InvalidateProgrammeCaches(assignedProgram.Id, assignedProgram.AthleteId);
            logger.LogInformation("Ungrouped exercises from superset {SupersetGroupId}, invalidated programme caches", supersetGroupId);
        }

        return result;
    }

    public async Task<IEnumerable<ExerciseSetResponse>?> CreateDropSetSequenceAsync(Guid workoutExerciseId, CreateDropSetRequest request)
    {
        var result = await innerService.CreateDropSetSequenceAsync(workoutExerciseId, request);

        if (result != null)
        {
            // Invalidate the workout exercise cache since sets were added
            await InvalidateWorkoutExerciseCache(workoutExerciseId);
            await InvalidateProgrammeCacheByWorkoutExerciseId(workoutExerciseId);
        }

        logger.LogInformation("Created drop set sequence for workout exercise {WorkoutExerciseId}, invalidated caches", workoutExerciseId);

        return result;
    }

    #endregion

    #region Helper Methods

    private async Task InvalidateWorkoutDayCache(Guid workoutDayId)
    {
        await cacheProvider.RemoveAsync(CacheKeys.WorkoutDayById(workoutDayId));
    }

    private async Task InvalidateWorkoutExerciseCache(Guid workoutExerciseId)
    {
        await cacheProvider.RemoveAsync(CacheKeys.WorkoutExerciseById(workoutExerciseId));
    }

    private async Task InvalidateExerciseSetCache(Guid exerciseSetId)
    {
        await cacheProvider.RemoveAsync(CacheKeys.ExerciseSetById(exerciseSetId));
    }

    /// <summary>
    /// Invalidates all programme-related caches for the given programme and athlete.
    /// This ensures the frontend receives fresh data when querying assigned programmes.
    /// </summary>
    private async Task InvalidateProgrammeCaches(Guid programmeId, Guid athleteId)
    {
        // Invalidate the specific programme cache
        await cacheProvider.RemoveAsync(CacheKeys.ProgrammeById(programmeId));

        // Invalidate athlete's programme list cache
        await cacheProvider.RemoveAsync(CacheKeys.ProgrammesByAthlete(athleteId));

        // Invalidate active programme cache for this athlete
        await cacheProvider.RemoveAsync(CacheKeys.ActiveProgrammeByAthlete(athleteId));

        logger.LogDebug("Invalidated programme caches for programme {ProgrammeId} and athlete {AthleteId}",
            programmeId, athleteId);
    }

    /// <summary>
    /// Helper to invalidate programme caches by looking up the programme from a workout day ID.
    /// </summary>
    private async Task InvalidateProgrammeCacheByWorkoutDayId(Guid workoutDayId)
    {
        var assignedProgram = await assignedProgramRepository.GetByWorkoutDayIdAsync(workoutDayId);
        if (assignedProgram != null)
        {
            await InvalidateProgrammeCaches(assignedProgram.Id, assignedProgram.AthleteId);
        }
    }

    /// <summary>
    /// Helper to invalidate programme caches by looking up the programme from a workout exercise ID.
    /// </summary>
    private async Task InvalidateProgrammeCacheByWorkoutExerciseId(Guid workoutExerciseId)
    {
        var assignedProgram = await assignedProgramRepository.GetByWorkoutExerciseIdAsync(workoutExerciseId);
        if (assignedProgram != null)
        {
            await InvalidateProgrammeCaches(assignedProgram.Id, assignedProgram.AthleteId);
        }
    }

    /// <summary>
    /// Helper to invalidate programme caches by looking up the programme from an exercise set ID.
    /// </summary>
    private async Task InvalidateProgrammeCacheByExerciseSetId(Guid exerciseSetId)
    {
        var assignedProgram = await assignedProgramRepository.GetByExerciseSetIdAsync(exerciseSetId);
        if (assignedProgram != null)
        {
            await InvalidateProgrammeCaches(assignedProgram.Id, assignedProgram.AthleteId);
        }
    }

    /// <summary>
    /// Helper to invalidate programme caches by looking up the programme from a week ID.
    /// </summary>
    private async Task InvalidateProgrammeCacheByWeekId(Guid weekId)
    {
        var assignedProgram = await assignedProgramRepository.GetByWeekIdAsync(weekId);
        if (assignedProgram != null)
        {
            await InvalidateProgrammeCaches(assignedProgram.Id, assignedProgram.AthleteId);
        }
    }

    #endregion
}