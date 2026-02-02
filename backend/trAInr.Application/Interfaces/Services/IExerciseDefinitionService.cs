using trAInr.Application.DTOs;
using trAInr.Domain.Entities;

namespace trAInr.Application.Interfaces.Services;

public interface IExerciseDefinitionService
{
    Task<ExerciseResponse?> GetByIdAsync(int id);
    Task<IEnumerable<ExerciseResponse>> GetAllAsync();
    Task<PagedResponse<ExerciseResponse>> GetAllPagedAsync(PagedRequest request);
    Task<IEnumerable<ExerciseSummaryResponse>> SearchAsync(string? query, ExerciseType? type, MuscleGroup? muscleGroup);
    Task<PagedResponse<ExerciseSummaryResponse>> SearchPagedAsync(string? query, ExerciseType? type, MuscleGroup? muscleGroup, PagedRequest request);
    Task<ExerciseResponse> CreateAsync(CreateExerciseRequest request, Guid? userId = null);
    Task<ExerciseResponse?> UpdateAsync(int id, UpdateExerciseRequest request);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<ExerciseSummaryResponse>> GetByTypeAsync(ExerciseType type);
    Task<IEnumerable<ExerciseSummaryResponse>> GetByMuscleGroupAsync(MuscleGroup muscleGroup);
}