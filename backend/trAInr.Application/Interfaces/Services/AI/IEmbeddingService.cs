namespace trAInr.Application.Interfaces.Services.AI;

public interface IEmbeddingService
{
    Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default);
    Task GenerateExerciseEmbeddingsAsync(CancellationToken cancellationToken = default);
    Task<float[]> GenerateQueryEmbeddingAsync(string goal, string? targetFocus = null, CancellationToken cancellationToken = default);
}
