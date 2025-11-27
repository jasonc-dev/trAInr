namespace trAInr.API.Models.DTOs;

public record AddWorkoutExerciseRequest(
    Guid ExerciseId,
    int OrderIndex,
    string? Notes,
    int TargetSets,
    int TargetReps,
    decimal? TargetWeight,
    int? TargetDurationSeconds,
    decimal? TargetDistance);

public record UpdateWorkoutExerciseRequest(
    int OrderIndex,
    string? Notes,
    int TargetSets,
    int TargetReps,
    decimal? TargetWeight,
    int? TargetDurationSeconds,
    decimal? TargetDistance);

public record WorkoutExerciseResponse(
    Guid Id,
    Guid ExerciseId,
    string ExerciseName,
    int OrderIndex,
    string? Notes,
    int TargetSets,
    int TargetReps,
    decimal? TargetWeight,
    int? TargetDurationSeconds,
    decimal? TargetDistance,
    IEnumerable<ExerciseSetResponse> Sets);

public record WorkoutExerciseSummaryResponse(
    Guid Id,
    string ExerciseName,
    int OrderIndex,
    int TargetSets,
    int CompletedSets);

