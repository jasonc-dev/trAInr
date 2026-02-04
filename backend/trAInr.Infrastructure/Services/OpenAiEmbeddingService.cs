using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using trAInr.Application.Interfaces.Services.AI;
using trAInr.Domain.Aggregates;
using trAInr.Domain.Entities;
using trAInr.Domain.Enums;
using trAInr.Infrastructure.Data;

namespace trAInr.Infrastructure.Services;

public class OpenAiEmbeddingService : IEmbeddingService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly TrainrDbContext _context;
    private readonly ILogger<OpenAiEmbeddingService> _logger;
    private const string EmbeddingModel = "text-embedding-3-small";
    private const int EmbeddingDimensions = 1536;
    private const long CurrentCatalogVersion = 1;

    public OpenAiEmbeddingService(
        HttpClient httpClient,
        IConfiguration configuration,
        TrainrDbContext context,
        ILogger<OpenAiEmbeddingService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _context = context;
        _logger = logger;

        var apiKey = _configuration["OpenAi:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
            throw new InvalidOperationException("OpenAI API key is not configured");

        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        var request = new
        {
            model = EmbeddingModel,
            input = text,
            dimensions = EmbeddingDimensions
        };

        var response = await _httpClient.PostAsJsonAsync(
            "https://api.openai.com/v1/embeddings",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<EmbeddingResponse>(cancellationToken);

        if (result?.Data == null || !result.Data.Any())
            throw new InvalidOperationException("No embedding returned from OpenAI");

        return result.Data[0].Embedding;
    }

    public async Task<float[]> GenerateQueryEmbeddingAsync(
        string goal,
        string? targetFocus = null,
        CancellationToken cancellationToken = default)
    {
        var queryText = string.IsNullOrWhiteSpace(targetFocus)
            ? $"{goal} workout program"
            : $"{goal} workout program focusing on {targetFocus}";

        return await GenerateEmbeddingAsync(queryText, cancellationToken);
    }

    public async Task GenerateExerciseEmbeddingsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting exercise embedding generation...");

        // Get all exercises that don't have embeddings or have outdated embeddings
        var exercises = await _context.ExerciseDefinitions
            .Include(e => e.ExerciseEquipments).ThenInclude(ee => ee.Equipment)
            .Include(e => e.ExerciseMuscles).ThenInclude(em => em.Muscle)
            .Include(e => e.ExerciseTags).ThenInclude(et => et.Tag)
            .Where(e => e.IsSystemExercise)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Found {Count} exercises to process", exercises.Count);

        var processedCount = 0;
        var batchSize = 10;

        // Process in batches to avoid rate limits
        for (var i = 0; i < exercises.Count; i += batchSize)
        {
            var batch = exercises.Skip(i).Take(batchSize).ToList();

            foreach (var exercise in batch)
            {
                try
                {
                    // Check if embedding already exists
                    var existingEmbedding = await _context.ExerciseEmbeddings
                        .FirstOrDefaultAsync(e => e.ExerciseDefinitionId == exercise.Id, cancellationToken);

                    if (existingEmbedding is { CatalogVersion: >= CurrentCatalogVersion })
                    {
                        _logger.LogDebug("Skipping {ExerciseName} - embedding is up to date", exercise.Name);
                        continue;
                    }

                    // Compose embedding text
                    var embeddingText = ComposeEmbeddingText(exercise);

                    // Generate embedding
                    var embeddingVector = await GenerateEmbeddingAsync(embeddingText, cancellationToken);

                    // Save or update embedding
                    if (existingEmbedding == null)
                    {
                        var newEmbedding = ExerciseEmbedding.Create(
                            exercise.Id,
                            embeddingVector,
                            EmbeddingModel,
                            CurrentCatalogVersion);

                        await _context.ExerciseEmbeddings.AddAsync(newEmbedding, cancellationToken);
                    }
                    else
                    {
                        existingEmbedding.Update(embeddingVector, CurrentCatalogVersion);
                    }

                    processedCount++;
                    _logger.LogInformation("Generated embedding for {ExerciseName} ({Count}/{Total})",
                        exercise.Name, processedCount, exercises.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to generate embedding for exercise {ExerciseName}", exercise.Name);
                }
            }

            // Save batch
            await _context.SaveChangesAsync(cancellationToken);

            // Rate limiting delay (OpenAI has rate limits)
            if (i + batchSize < exercises.Count)
            {
                await Task.Delay(1000, cancellationToken);
            }
        }

        _logger.LogInformation("Completed embedding generation. Processed {Count} exercises", processedCount);
    }

    private static string ComposeEmbeddingText(ExerciseDefinition exercise)
    {
        var primaryMuscles = exercise.ExerciseMuscles
            .Where(em => em.Role == MuscleRole.Primary)
            .Select(em => em.Muscle.Name)
            .ToList();

        var secondaryMuscles = exercise.ExerciseMuscles
            .Where(em => em.Role == MuscleRole.Secondary)
            .Select(em => em.Muscle.Name)
            .ToList();

        var equipment = exercise.ExerciseEquipments
            .Select(ee => ee.Equipment.Name)
            .ToList();

        var tags = exercise.ExerciseTags
            .Select(et => et.Tag.Name)
            .ToList();

        var parts = new List<string>
        {
            $"{exercise.Name}",
            $"{exercise.MovementPattern} {exercise.Type} exercise"
        };

        if (primaryMuscles.Any())
            parts.Add($"targeting {string.Join(", ", primaryMuscles)}");

        if (secondaryMuscles.Any())
            parts.Add($"with secondary focus on {string.Join(", ", secondaryMuscles)}");

        if (equipment.Any())
            parts.Add($"using {string.Join(", ", equipment)}");

        parts.Add($"difficulty level {exercise.LevelOfDifficulty}");

        if (tags.Any())
            parts.Add($"tags: {string.Join(", ", tags)}");

        if (!string.IsNullOrWhiteSpace(exercise.Description))
            parts.Add(exercise.Description);

        return string.Join(". ", parts);
    }

    private class EmbeddingResponse
    {
        public List<EmbeddingData> Data { get; set; } = new();
    }

    private class EmbeddingData
    {
        public float[] Embedding { get; set; } = Array.Empty<float>();
    }
}
