using Pgvector;
using trAInr.Domain.Aggregates;

namespace trAInr.Domain.Entities;

public class ExerciseEmbedding
{
    public int ExerciseDefinitionId { get; private set; }
    public Vector Embedding { get; private set; } = null!;
    public string EmbeddingModel { get; private set; } = string.Empty;
    public long CatalogVersion { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Navigation property
    public ExerciseDefinition ExerciseDefinition { get; private set; } = null!;

    private ExerciseEmbedding() { } // For EF Core

    public static ExerciseEmbedding Create(
        int exerciseDefinitionId,
        float[] embeddingVector,
        string embeddingModel,
        long catalogVersion)
    {
        if (embeddingVector == null || embeddingVector.Length == 0)
            throw new ArgumentException("Embedding vector cannot be empty", nameof(embeddingVector));

        if (string.IsNullOrWhiteSpace(embeddingModel))
            throw new ArgumentException("Embedding model cannot be empty", nameof(embeddingModel));

        return new ExerciseEmbedding
        {
            ExerciseDefinitionId = exerciseDefinitionId,
            Embedding = new Vector(embeddingVector),
            EmbeddingModel = embeddingModel,
            CatalogVersion = catalogVersion,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(float[] embeddingVector, long catalogVersion)
    {
        if (embeddingVector == null || embeddingVector.Length == 0)
            throw new ArgumentException("Embedding vector cannot be empty", nameof(embeddingVector));

        Embedding = new Vector(embeddingVector);
        CatalogVersion = catalogVersion;
        UpdatedAt = DateTime.UtcNow;
    }
}
