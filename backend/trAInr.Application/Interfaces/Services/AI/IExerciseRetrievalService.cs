using trAInr.Application.DTOs.AI;

namespace trAInr.Application.Interfaces.Services.AI;

public interface IExerciseRetrievalService
{
    Task<IReadOnlyList<ExercisePromptDto>> GetCandidateExercisesAsync(
        ExerciseRetrievalRequest request,
        CancellationToken cancellationToken = default);
}
