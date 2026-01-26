using trAInr.Domain.Entities;

namespace trAInr.Application.DTOs;

public record CreateExerciseRequest(
    string Name,
    string Description,
    ExerciseType Type,
    MuscleGroup PrimaryMuscleGroup,
    MuscleGroup? SecondaryMuscleGroup,
    LevelOfDifficulty LevelOfDifficulty,
    string? Instructions,
    string? VideoUrl);

public record UpdateExerciseRequest(
    string Name,
    string Description,
    string? Instructions,
    string? VideoUrl);

public record ExerciseResponse(
    int Id,
    string Name,
    string Description,
    ExerciseType Type,
    MuscleGroup PrimaryMuscleGroup,
    MuscleGroup? SecondaryMuscleGroup,
    string? Instructions,
    string? VideoUrl,
    bool IsSystemExercise);

public record ExerciseSummaryResponse(
    int Id,
    string Name,
    ExerciseType Type,
    MuscleGroup PrimaryMuscleGroup,
    MuscleGroup? SecondaryMuscleGroup);
