using Microsoft.Extensions.Logging;
using trAInr.Application.DTOs;
using trAInr.Application.Interfaces;
using trAInr.Application.Interfaces.Repositories;
using trAInr.Application.Interfaces.Services;
using trAInr.Domain.Aggregates;
using trAInr.Domain.Entities;

namespace trAInr.Application.Services;

public class ExerciseDefinitionService(
    IExerciseDefinitionRepository exerciseDefinitionRepository,
    IUnitOfWork unitOfWork,
    ILogger<ExerciseDefinitionService> logger)
    : IExerciseDefinitionService
{
    public async Task<ExerciseResponse?> GetByIdAsync(int id)
    {
        var exerciseDefinition = await exerciseDefinitionRepository.GetByIdAsync(id);
        return exerciseDefinition is null ? null : MapToResponse(exerciseDefinition);
    }

    public async Task<IEnumerable<ExerciseResponse>> GetAllAsync()
    {
        var exerciseDefinitions = await exerciseDefinitionRepository.GetAllAsync();
        return exerciseDefinitions.Select(MapToResponse);
    }

    public async Task<IEnumerable<ExerciseSummaryResponse>> SearchAsync(
        string? query,
        ExerciseType? type,
        MuscleGroup? muscleGroup)
    {
        var exerciseDefinitions = await exerciseDefinitionRepository.SearchAsync(query, type, muscleGroup);
        return exerciseDefinitions.Select(e => new ExerciseSummaryResponse(
            e.Id,
            e.Name,
            e.Type,
            e.PrimaryMuscleGroup,
            e.SecondaryMuscleGroup));
    }

    public async Task<ExerciseResponse> CreateAsync(CreateExerciseRequest request, Guid? userId = null)
    {
        // Map ExerciseType to MovementPattern (simplified mapping)
        var movementPattern = MapExerciseTypeToMovementPattern(request.Type);

        // Get next available ID (since we're using int now, need to get max + 1)
        var allExercises = await exerciseDefinitionRepository.GetAllAsync();
        var nextId = allExercises.Any() ? allExercises.Max(e => e.Id) + 1 : 1;

        var exerciseDefinition = new ExerciseDefinition(
            nextId,
            request.Name,
            request.Description,
            request.Type,
            movementPattern,
            request.PrimaryMuscleGroup,
            request.SecondaryMuscleGroup,
            request.LevelOfDifficulty,
            request.Instructions,
            request.VideoUrl,
            isSystemExercise: false,
            userId);

        await exerciseDefinitionRepository.AddAsync(exerciseDefinition);
        await unitOfWork.SaveChangesAsync();

        return MapToResponse(exerciseDefinition);
    }

    public async Task<ExerciseResponse?> UpdateAsync(int id, UpdateExerciseRequest request)
    {
        var exerciseDefinition = await exerciseDefinitionRepository.GetByIdAsync(id);
        if (exerciseDefinition is null) return null;

        try
        {
            exerciseDefinition.Update(
                request.Name,
                request.Description,
                request.Instructions,
                request.VideoUrl);

            await exerciseDefinitionRepository.UpdateAsync(exerciseDefinition);
            await unitOfWork.SaveChangesAsync();

            return MapToResponse(exerciseDefinition);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Attempted to update system exercise {ExerciseId}", id);
            return null;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var exerciseDefinition = await exerciseDefinitionRepository.GetByIdAsync(id);
        if (exerciseDefinition is null || exerciseDefinition.IsSystemExercise) return false;

        await exerciseDefinitionRepository.DeleteAsync(exerciseDefinition);
        await unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<ExerciseSummaryResponse>> GetByTypeAsync(ExerciseType type)
    {
        var exerciseDefinitions = await exerciseDefinitionRepository.GetByTypeAsync(type);
        return exerciseDefinitions.Select(e => new ExerciseSummaryResponse(
            e.Id,
            e.Name,
            e.Type,
            e.PrimaryMuscleGroup,
            e.SecondaryMuscleGroup));
    }

    public async Task<IEnumerable<ExerciseSummaryResponse>> GetByMuscleGroupAsync(MuscleGroup muscleGroup)
    {
        var exerciseDefinitions = await exerciseDefinitionRepository.GetByMuscleGroupAsync(muscleGroup);
        return exerciseDefinitions.Select(e => new ExerciseSummaryResponse(
            e.Id,
            e.Name,
            e.Type,
            e.PrimaryMuscleGroup,
            e.SecondaryMuscleGroup));
    }

    private static MovementPattern MapExerciseTypeToMovementPattern(ExerciseType type)
    {
        return type switch
        {
            ExerciseType.WeightTraining => MovementPattern.Push, // Default, could be more sophisticated
            ExerciseType.Cardio => MovementPattern.Cardio,
            ExerciseType.Bodyweight => MovementPattern.Push,
            ExerciseType.Flexibility => MovementPattern.Flexibility,
            _ => MovementPattern.Push
        };
    }

    private static ExerciseResponse MapToResponse(ExerciseDefinition exerciseDefinition)
    {
        return new ExerciseResponse(
            exerciseDefinition.Id,
            exerciseDefinition.Name,
            exerciseDefinition.Description,
            exerciseDefinition.Type,
            exerciseDefinition.PrimaryMuscleGroup,
            exerciseDefinition.SecondaryMuscleGroup,
            exerciseDefinition.Instructions,
            exerciseDefinition.VideoUrl,
            exerciseDefinition.IsSystemExercise);
    }
}