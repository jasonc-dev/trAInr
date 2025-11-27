using Microsoft.EntityFrameworkCore;
using trAInr.API.Data;
using trAInr.API.Models.Domain;
using trAInr.API.Models.DTOs;

namespace trAInr.API.Services;

public class ExerciseService : IExerciseService
{
    private readonly TrainrDbContext _context;

    public ExerciseService(TrainrDbContext context)
    {
        _context = context;
    }

    public async Task<ExerciseResponse?> GetByIdAsync(Guid id)
    {
        var exercise = await _context.Exercises.FindAsync(id);
        return exercise is null ? null : MapToResponse(exercise);
    }

    public async Task<IEnumerable<ExerciseResponse>> GetAllAsync()
    {
        return await _context.Exercises
            .OrderBy(e => e.Name)
            .Select(e => MapToResponse(e))
            .ToListAsync();
    }

    public async Task<IEnumerable<ExerciseSummaryResponse>> SearchAsync(
        string? query, 
        ExerciseType? type, 
        MuscleGroup? muscleGroup)
    {
        var queryable = _context.Exercises.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var lowerQuery = query.ToLower();
            queryable = queryable.Where(e => 
                e.Name.ToLower().Contains(lowerQuery) || 
                e.Description.ToLower().Contains(lowerQuery));
        }

        if (type.HasValue)
        {
            queryable = queryable.Where(e => e.Type == type.Value);
        }

        if (muscleGroup.HasValue)
        {
            queryable = queryable.Where(e => 
                e.PrimaryMuscleGroup == muscleGroup.Value || 
                e.SecondaryMuscleGroup == muscleGroup.Value);
        }

        return await queryable
            .OrderBy(e => e.Name)
            .Select(e => new ExerciseSummaryResponse(
                e.Id,
                e.Name,
                e.Type,
                e.PrimaryMuscleGroup))
            .ToListAsync();
    }

    public async Task<ExerciseResponse> CreateAsync(CreateExerciseRequest request, Guid? userId = null)
    {
        var exercise = new Exercise
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Type = request.Type,
            PrimaryMuscleGroup = request.PrimaryMuscleGroup,
            SecondaryMuscleGroup = request.SecondaryMuscleGroup,
            Instructions = request.Instructions,
            VideoUrl = request.VideoUrl,
            IsSystemExercise = userId is null,
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Exercises.Add(exercise);
        await _context.SaveChangesAsync();

        return MapToResponse(exercise);
    }

    public async Task<ExerciseResponse?> UpdateAsync(Guid id, UpdateExerciseRequest request)
    {
        var exercise = await _context.Exercises.FindAsync(id);
        if (exercise is null) return null;

        exercise.Name = request.Name;
        exercise.Description = request.Description;
        exercise.Instructions = request.Instructions;
        exercise.VideoUrl = request.VideoUrl;

        await _context.SaveChangesAsync();
        return MapToResponse(exercise);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var exercise = await _context.Exercises.FindAsync(id);
        if (exercise is null || exercise.IsSystemExercise) return false;

        _context.Exercises.Remove(exercise);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<ExerciseSummaryResponse>> GetByTypeAsync(ExerciseType type)
    {
        return await _context.Exercises
            .Where(e => e.Type == type)
            .OrderBy(e => e.Name)
            .Select(e => new ExerciseSummaryResponse(
                e.Id,
                e.Name,
                e.Type,
                e.PrimaryMuscleGroup))
            .ToListAsync();
    }

    public async Task<IEnumerable<ExerciseSummaryResponse>> GetByMuscleGroupAsync(MuscleGroup muscleGroup)
    {
        return await _context.Exercises
            .Where(e => e.PrimaryMuscleGroup == muscleGroup || e.SecondaryMuscleGroup == muscleGroup)
            .OrderBy(e => e.Name)
            .Select(e => new ExerciseSummaryResponse(
                e.Id,
                e.Name,
                e.Type,
                e.PrimaryMuscleGroup))
            .ToListAsync();
    }

    private static ExerciseResponse MapToResponse(Exercise e) =>
        new(
            e.Id,
            e.Name,
            e.Description,
            e.Type,
            e.PrimaryMuscleGroup,
            e.SecondaryMuscleGroup,
            e.Instructions,
            e.VideoUrl,
            e.IsSystemExercise);
}

