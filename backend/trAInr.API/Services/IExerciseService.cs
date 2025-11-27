using trAInr.API.Models.Domain;
using trAInr.API.Models.DTOs;

namespace trAInr.API.Services;

public interface IExerciseService
{
    Task<ExerciseResponse?> GetByIdAsync(Guid id);
    Task<IEnumerable<ExerciseResponse>> GetAllAsync();
    Task<IEnumerable<ExerciseSummaryResponse>> SearchAsync(string? query, ExerciseType? type, MuscleGroup? muscleGroup);
    Task<ExerciseResponse> CreateAsync(CreateExerciseRequest request, Guid? userId = null);
    Task<ExerciseResponse?> UpdateAsync(Guid id, UpdateExerciseRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<ExerciseSummaryResponse>> GetByTypeAsync(ExerciseType type);
    Task<IEnumerable<ExerciseSummaryResponse>> GetByMuscleGroupAsync(MuscleGroup muscleGroup);
}

