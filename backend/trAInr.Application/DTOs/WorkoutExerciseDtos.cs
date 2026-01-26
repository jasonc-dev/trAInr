namespace trAInr.Application.DTOs;

public record AddWorkoutExerciseRequest(
    int ExerciseId,
    int OrderIndex,
    string? Notes,
    int TargetSets,
    int TargetReps,
    decimal? TargetWeight,
    int? TargetDurationSeconds,
    decimal? TargetDistance,
    int? RestSeconds,
    int? TargetRpe,
    Guid? SupersetGroupId,
    int? SupersetRestSeconds);

public record UpdateWorkoutExerciseRequest(
    int OrderIndex,
    string? Notes,
    int TargetSets,
    int TargetReps,
    decimal? TargetWeight,
    int? TargetDurationSeconds,
    decimal? TargetDistance,
    int? RestSeconds,
    int? TargetRpe,
    Guid? SupersetGroupId,
    int? SupersetRestSeconds);

public record WorkoutExerciseResponse(
    Guid Id,
    int ExerciseId,
    string ExerciseName,
    int OrderIndex,
    string? Notes,
    int TargetSets,
    int TargetReps,
    decimal? TargetWeight,
    int? TargetDurationSeconds,
    decimal? TargetDistance,
    int? RestSeconds,
    int? TargetRpe,
    Guid? SupersetGroupId,
    int? SupersetRestSeconds,
    IEnumerable<ExerciseSetResponse> Sets);

public record WorkoutExerciseSummaryResponse(
    Guid Id,
    string ExerciseName,
    int OrderIndex,
    int TargetSets,
    int CompletedSets);

/// <summary>
/// Request to group multiple exercises into a superset
/// </summary>
public record GroupSupersetRequest(
    List<Guid> ExerciseIds,
    int? SupersetRestSeconds);
