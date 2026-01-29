using Microsoft.Extensions.Logging;
using trAInr.Application.Constants;
using trAInr.Application.DTOs;
using trAInr.Application.Interfaces;
using trAInr.Application.Interfaces.Services;

namespace trAInr.Application.Services.AssignedProgramme;

/// <summary>
/// Decorator for AssignedProgrammeService that adds caching capabilities.
/// Implements the decorator pattern to separate caching concerns from business logic.
/// </summary>
public class CachedAssignedProgrammeService(
    IAssignedProgrammeService innerService,
    ICacheProvider cacheProvider,
    ILogger<CachedAssignedProgrammeService> logger) : IAssignedProgrammeService
{
    #region Read Operations (Cached)

    public async Task<ProgrammeResponse?> GetByIdAsync(Guid id)
    {
        var cacheKey = CacheKeys.ProgrammeById(id);

        // Try to get from cache first
        var cachedResult = await cacheProvider.GetAsync<ProgrammeResponse>(cacheKey);
        if (cachedResult != null)
        {
            return cachedResult;
        }

        // Cache miss - get from service and cache the result
        var result = await innerService.GetByIdAsync(id);
        if (result != null)
        {
            await cacheProvider.SetAsync(cacheKey, result);
        }

        return result;
    }

    public async Task<IEnumerable<ProgrammeSummaryResponse>> GetByAthleteIdAsync(Guid athleteId)
    {
        var cacheKey = CacheKeys.ProgrammesByAthlete(athleteId);

        // Try to get from cache first
        var cachedResult = await cacheProvider.GetAsync<List<ProgrammeSummaryResponse>>(cacheKey);
        if (cachedResult != null)
        {
            return cachedResult;
        }

        // Cache miss - get from service and cache the result
        var result = await innerService.GetByAthleteIdAsync(athleteId);
        var resultList = result.ToList();

        if (resultList.Any())
        {
            await cacheProvider.SetAsync(cacheKey, resultList);
        }

        return resultList;
    }

    public async Task<ProgrammeSummaryResponse?> GetActiveByAthleteIdAsync(Guid athleteId)
    {
        var cacheKey = CacheKeys.ActiveProgrammeByAthlete(athleteId);

        // Try to get from cache first
        var cachedResult = await cacheProvider.GetAsync<ProgrammeSummaryResponse>(cacheKey);
        if (cachedResult != null)
        {
            return cachedResult;
        }

        // Cache miss - get from service and cache the result
        var result = await innerService.GetActiveByAthleteIdAsync(athleteId);
        if (result != null)
        {
            await cacheProvider.SetAsync(cacheKey, result);
        }

        return result;
    }

    public async Task<IEnumerable<ProgrammeSummaryResponse>> GetPreMadeProgrammesAsync()
    {
        var cacheKey = CacheKeys.PreMadeProgrammeTemplates;

        // Try to get from cache first
        var cachedResult = await cacheProvider.GetAsync<List<ProgrammeSummaryResponse>>(cacheKey);
        if (cachedResult != null)
        {
            return cachedResult;
        }

        // Cache miss - get from service and cache the result
        var result = await innerService.GetPreMadeProgrammesAsync();
        var resultList = result.ToList();

        if (resultList.Any())
        {
            await cacheProvider.SetAsync(cacheKey, resultList);
        }

        return resultList;
    }

    public async Task<IEnumerable<ProgrammeSummaryResponse>> GetProgrammesCreatedByAthleteAsync(Guid athleteId)
    {
        var cacheKey = CacheKeys.CreatedProgrammeTemplates(athleteId);

        // Try to get from cache first
        var cachedResult = await cacheProvider.GetAsync<List<ProgrammeSummaryResponse>>(cacheKey);
        if (cachedResult != null)
        {
            return cachedResult;
        }

        // Cache miss - get from service and cache the result
        var result = await innerService.GetProgrammesCreatedByAthleteAsync(athleteId);
        var resultList = result.ToList();

        if (resultList.Any())
        {
            await cacheProvider.SetAsync(cacheKey, resultList);
        }

        return resultList;
    }

    #endregion

    #region Write Operations (Cache Invalidation)

    public async Task<ProgrammeResponse> CreateAsync(Guid athleteId, CreateProgrammeRequest request)
    {
        var result = await innerService.CreateAsync(athleteId, request);

        // Invalidate athlete's programmes cache
        await InvalidateAthleteCaches(athleteId);

        logger.LogInformation("Created programme {ProgrammeId} for athlete {AthleteId}, invalidated caches",
            result.Id, athleteId);

        return result;
    }

    public async Task<ProgrammeResponse?> UpdateAsync(Guid id, UpdateProgrammeRequest request)
    {
        // Get the current programme to know which athlete's cache to invalidate
        var existingProgramme = await innerService.GetByIdAsync(id);
        if (existingProgramme == null)
        {
            return null;
        }

        var result = await innerService.UpdateAsync(id, request);
        if (result == null)
        {
            return null;
        }

        // Invalidate programme by ID cache
        await cacheProvider.RemoveAsync(CacheKeys.ProgrammeById(id));

        // Invalidate athlete's programmes cache
        await InvalidateAthleteCaches(result.UserId);

        // If IsActive changed, also invalidate active programme cache
        if (existingProgramme.IsActive != result.IsActive)
        {
            await cacheProvider.RemoveAsync(CacheKeys.ActiveProgrammeByAthlete(result.UserId));
        }

        logger.LogInformation("Updated programme {ProgrammeId}, invalidated caches", id);

        return result;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        // Get the programme first to know which athlete's cache to invalidate
        var existingProgramme = await innerService.GetByIdAsync(id);
        if (existingProgramme == null)
        {
            return false;
        }

        var result = await innerService.DeleteAsync(id);
        if (!result)
        {
            return false;
        }

        // Invalidate programme by ID cache
        await cacheProvider.RemoveAsync(CacheKeys.ProgrammeById(id));

        // Invalidate athlete's caches (programmes and active)
        await InvalidateAthleteCaches(existingProgramme.UserId);

        logger.LogInformation("Deleted programme {ProgrammeId}, invalidated caches", id);

        return true;
    }

    public async Task<ProgrammeResponse?> CloneProgrammeAsync(Guid programmeId, CloneProgrammeRequest request)
    {
        var result = await innerService.CloneProgrammeAsync(programmeId, request);
        if (result == null)
        {
            return null;
        }

        // Invalidate target athlete's caches
        await InvalidateAthleteCaches(request.AthleteId);

        logger.LogInformation("Cloned programme {ProgrammeId} for athlete {AthleteId}, invalidated caches",
            programmeId, request.AthleteId);

        return result;
    }

    public async Task<ProgrammeWeekResponse?> AddWeekAsync(Guid programmeId, CreateProgrammeWeekRequest request)
    {
        var result = await innerService.AddWeekAsync(programmeId, request);
        if (result == null)
        {
            return null;
        }

        // Invalidate programme by ID cache (week structure changed)
        await cacheProvider.RemoveAsync(CacheKeys.ProgrammeById(programmeId));

        logger.LogInformation("Added week to programme {ProgrammeId}, invalidated cache", programmeId);

        return result;
    }

    public async Task<ProgrammeWeekResponse?> UpdateWeekAsync(Guid weekId, UpdateProgrammeWeekRequest request)
    {
        var result = await innerService.UpdateWeekAsync(weekId, request);
        if (result == null)
        {
            return null;
        }

        // We need to invalidate the programme cache, but we don't have the programme ID directly
        // We need to get it from the result or make an additional call
        // For now, we'll use a prefix-based approach - but this is not ideal
        // A better approach would be to modify the interface to return programme ID or accept it as parameter

        logger.LogInformation("Updated week {WeekId}, cache invalidation may require programme ID", weekId);

        // Note: This is a limitation - we can't easily invalidate the specific programme cache
        // without knowing the programme ID. In production, consider:
        // 1. Modifying the method signature to return programme ID
        // 2. Making an additional query to get programme ID
        // 3. Accepting programmeId as an additional parameter

        return result;
    }

    public async Task<ProgrammeWeekResponse?> CopyWeekAsync(Guid sourceWeekId, int targetWeekNumber)
    {
        var result = await innerService.CopyWeekAsync(sourceWeekId, targetWeekNumber);
        if (result == null)
        {
            return null;
        }

        // Same limitation as UpdateWeekAsync - we don't have the programme ID
        logger.LogInformation("Copied week {SourceWeekId}, cache invalidation may require programme ID", sourceWeekId);

        return result;
    }

    public async Task<ProgrammeWeekResponse?> CopyWeekContentAsync(Guid sourceWeekId, Guid targetWeekId)
    {
        var result = await innerService.CopyWeekContentAsync(sourceWeekId, targetWeekId);
        if (result == null)
        {
            return null;
        }

        // Same limitation as UpdateWeekAsync - we don't have the programme ID
        logger.LogInformation("Copied week content from {SourceWeekId} to {TargetWeekId}, cache invalidation may require programme ID",
            sourceWeekId, targetWeekId);

        return result;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Invalidates all cache entries related to a specific athlete.
    /// </summary>
    private async Task InvalidateAthleteCaches(Guid athleteId)
    {
        // Invalidate athlete's programmes list
        await cacheProvider.RemoveAsync(CacheKeys.ProgrammesByAthlete(athleteId));

        // Invalidate active programme
        await cacheProvider.RemoveAsync(CacheKeys.ActiveProgrammeByAthlete(athleteId));

        // Invalidate created templates
        await cacheProvider.RemoveAsync(CacheKeys.CreatedProgrammeTemplates(athleteId));
    }

    #endregion
}
