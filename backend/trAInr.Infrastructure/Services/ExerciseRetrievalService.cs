using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using trAInr.Application.DTOs.AI;
using trAInr.Application.Interfaces.Services.AI;
using trAInr.Domain.Aggregates;
using trAInr.Domain.Entities;
using trAInr.Domain.Enums;
using trAInr.Infrastructure.Data;

namespace trAInr.Infrastructure.Services;

public class ExerciseRetrievalService : IExerciseRetrievalService
{
    private readonly TrainrDbContext _context;
    private readonly IEmbeddingService _embeddingService;
    private readonly ILogger<ExerciseRetrievalService> _logger;

    public ExerciseRetrievalService(
        TrainrDbContext context,
        IEmbeddingService embeddingService,
        ILogger<ExerciseRetrievalService> logger)
    {
        _context = context;
        _embeddingService = embeddingService;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ExercisePromptDto>> GetCandidateExercisesAsync(
        ExerciseRetrievalRequest request,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var useSemanticSearch = !string.IsNullOrWhiteSpace(request.Goal);

        List<ExerciseDefinition> exercises;

        if (useSemanticSearch)
        {
            _logger.LogInformation("Using semantic search for goal: {Goal}", request.Goal);
            exercises = await GetExercisesBySemanticSearchAsync(request, cancellationToken);

            if (exercises.Count == 0)
            {
                _logger.LogWarning("Semantic search returned no results, falling back to rule-based retrieval");
                exercises = await GetExercisesByRuleBasedFilteringAsync(request, cancellationToken);
            }
        }
        else
        {
            _logger.LogInformation("Using rule-based filtering (no goal provided)");
            exercises = await GetExercisesByRuleBasedFilteringAsync(request, cancellationToken);
        }

        // Apply equipment and tag filters (common to both paths)
        exercises = ApplyEquipmentFilters(exercises, request);
        exercises = ApplyTagFilters(exercises, request);

        // Apply diversity: limit per movement pattern to avoid returning 30 curls
        var diverseExercises = exercises
            .GroupBy(e => e.MovementPattern)
            .SelectMany(g => g.Take(request.PerMovementPatternLimit))
            .Take(request.Limit)
            .ToList();

        stopwatch.Stop();
        _logger.LogInformation(
            "Exercise retrieval completed in {ElapsedMs}ms. Method: {Method}, Candidates: {Count}, Final: {FinalCount}",
            stopwatch.ElapsedMilliseconds,
            useSemanticSearch ? "Semantic" : "Rule-based",
            exercises.Count,
            diverseExercises.Count);

        // Project to ExercisePromptDto
        return ProjectToDto(diverseExercises);
    }

    private async Task<List<ExerciseDefinition>> GetExercisesBySemanticSearchAsync(
        ExerciseRetrievalRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Check if embeddings exist
            var embeddingsExist = await _context.ExerciseEmbeddings.AnyAsync(cancellationToken);
            if (!embeddingsExist)
            {
                _logger.LogWarning("Goal provided but no exercise embeddings found. Generate embeddings via admin endpoint first.");
                return new List<ExerciseDefinition>();
            }

            // Generate query embedding from user's goal
            var queryEmbedding = await _embeddingService.GenerateQueryEmbeddingAsync(
                request.Goal!,
                request.TargetFocus,
                cancellationToken);

            var queryVector = new Vector(queryEmbedding);

            // Vector similarity search with base filters
            var semanticResults = await _context.ExerciseEmbeddings
                .Where(emb => emb.ExerciseDefinition.IsSystemExercise)
                .Where(emb => emb.ExerciseDefinition.LevelOfDifficulty <= request.MaxDifficulty)
                .Where(emb => emb.ExerciseDefinition.Type != ExerciseType.Flexibility)
                .OrderBy(emb => emb.Embedding.CosineDistance(queryVector))
                .Take(request.Limit * 2) // Get more for post-filtering
                .Include(emb => emb.ExerciseDefinition)
                    .ThenInclude(e => e.ExerciseEquipments).ThenInclude(ee => ee.Equipment)
                .Include(emb => emb.ExerciseDefinition)
                    .ThenInclude(e => e.ExerciseMuscles).ThenInclude(em => em.Muscle)
                .Include(emb => emb.ExerciseDefinition)
                    .ThenInclude(e => e.ExerciseTags).ThenInclude(et => et.Tag)
                .Select(emb => emb.ExerciseDefinition)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Semantic search returned {Count} exercises", semanticResults.Count);
            return semanticResults;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during semantic search, falling back to rule-based retrieval");
            return new List<ExerciseDefinition>();
        }
    }

    private async Task<List<ExerciseDefinition>> GetExercisesByRuleBasedFilteringAsync(
        ExerciseRetrievalRequest request,
        CancellationToken cancellationToken)
    {
        // Start with active exercises
        var query = _context.ExerciseDefinitions
            .Include(e => e.ExerciseEquipments).ThenInclude(ee => ee.Equipment)
            .Include(e => e.ExerciseMuscles).ThenInclude(em => em.Muscle)
            .Include(e => e.ExerciseTags).ThenInclude(et => et.Tag)
            .Where(e => e.IsSystemExercise)
            .AsQueryable();

        // Filter by difficulty
        query = query.Where(e => e.LevelOfDifficulty <= request.MaxDifficulty);

        // Exclude flexibility for program generation
        query = query.Where(e => e.Type != ExerciseType.Flexibility);

        // Fetch exercises into memory for complex filtering
        var exercises = await query.ToListAsync(cancellationToken);

        _logger.LogInformation("Rule-based filtering returned {Count} exercises", exercises.Count);
        return exercises;
    }

    private List<ExerciseDefinition> ApplyEquipmentFilters(
        List<ExerciseDefinition> exercises,
        ExerciseRetrievalRequest request)
    {
        if (request.AvailableEquipment.Length == 0)
            return exercises;

        var availableEquipmentSet = request.AvailableEquipment.ToHashSet(StringComparer.OrdinalIgnoreCase);

        var filtered = exercises.Where(e =>
        {
            var requiredEquipment = e.ExerciseEquipments.Select(ee => ee.Equipment.Name).ToList();

            // If no equipment specified or bodyweight, it's always available
            if (!requiredEquipment.Any() || e.IsBodyweight)
                return true;

            // Check if all required equipment is available
            return requiredEquipment.All(availableEquipmentSet.Contains);
        }).ToList();

        _logger.LogDebug("Equipment filter: {Before} -> {After} exercises", exercises.Count, filtered.Count);
        return filtered;
    }

    private List<ExerciseDefinition> ApplyTagFilters(
        List<ExerciseDefinition> exercises,
        ExerciseRetrievalRequest request)
    {
        // Apply excluded tags
        if (request.ExcludedTags?.Length > 0)
        {
            var excludedTagSet = request.ExcludedTags.ToHashSet(StringComparer.OrdinalIgnoreCase);
            exercises = exercises.Where(e =>
            {
                var exerciseTags = e.ExerciseTags.Select(et => $"{et.Tag.Namespace}:{et.Tag.Name}").ToList();
                return !exerciseTags.Any(tag => excludedTagSet.Contains(tag));
            }).ToList();

            _logger.LogDebug("Excluded tags filter: {Count} exercises remain", exercises.Count);
        }

        // Apply required tags
        if (request.RequiredTags?.Length > 0)
        {
            var requiredTagSet = request.RequiredTags.ToHashSet(StringComparer.OrdinalIgnoreCase);
            exercises = exercises.Where(e =>
            {
                var exerciseTags = e.ExerciseTags.Select(et => $"{et.Tag.Namespace}:{et.Tag.Name}").ToList();
                return requiredTagSet.All(tag => exerciseTags.Contains(tag));
            }).ToList();

            _logger.LogDebug("Required tags filter: {Count} exercises remain", exercises.Count);
        }

        return exercises;
    }

    private static List<ExercisePromptDto> ProjectToDto(List<ExerciseDefinition> exercises)
    {
        return exercises.Select(e => new ExercisePromptDto(
            Id: e.Id,
            Name: e.Name,
            MovementPattern: e.MovementPattern.ToString(),
            ExerciseType: e.Type.ToString(),
            Equipment: e.ExerciseEquipments.Select(ee => ee.Equipment.Name).ToArray(),
            PrimaryMuscles: e.ExerciseMuscles
                .Where(em => em.Role == MuscleRole.Primary)
                .Select(em => em.Muscle.Name)
                .ToArray(),
            SecondaryMuscles: e.ExerciseMuscles
                .Where(em => em.Role == MuscleRole.Secondary)
                .Select(em => em.Muscle.Name)
                .ToArray(),
            Difficulty: e.LevelOfDifficulty.ToString(),
            SpinalLoad: e.SpinalLoad.ToString(),
            SetupComplexity: e.SetupComplexity.ToString(),
            TrackingMode: e.TrackingMode.ToString(),
            DefaultRepRange: e is { DefaultRepMin: not null, DefaultRepMax: not null }
                ? $"{e.DefaultRepMin}-{e.DefaultRepMax}"
                : null,
            Tags: e.ExerciseTags.Select(et => $"{et.Tag.Namespace}:{et.Tag.Name}").ToArray(),
            ShortCue: e.ShortCue
        )).ToList();
    }
}
