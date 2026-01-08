using trAInr.Domain.Entities;

namespace trAInr.Application.DTOs;

public record CreateExerciseRequest(
    string Name,
    string Description,
    ExerciseType Type,
    MuscleGroup PrimaryMuscleGroup,
    MuscleGroup? SecondaryMuscleGroup,
    string? Instructions,
    string? VideoUrl);

public record UpdateExerciseRequest(
    string Name,
    string Description,
    string? Instructions,
    string? VideoUrl);

public record ExerciseResponse(
    Guid Id,
    string Name,
    string Description,
    ExerciseType Type,
    MuscleGroup PrimaryMuscleGroup,
    MuscleGroup? SecondaryMuscleGroup,
    string? Instructions,
    string? VideoUrl,
    bool IsSystemExercise);

public record ExerciseSummaryResponse(
    Guid Id,
    string Name,
    ExerciseType Type,
    MuscleGroup PrimaryMuscleGroup,
    MuscleGroup? SecondaryMuscleGroup);
