namespace trAInr.Application.DTOs.ProgramTemplate;

/// <summary>
///     Response DTO for a program template workout exercise
/// </summary>
public record ProgramTemplateWorkoutExerciseResponse(
    Guid Id,
    Guid ProgramTemplateWorkoutDayId,
    int ExerciseDefinitionId,
    int OrderIndex,
    string? Notes,
    int TargetSets,
    int TargetReps,
    decimal? TargetWeight,
    int? TargetDurationSeconds,
    decimal? TargetDistance,
    int? RestSeconds,
    Guid? SupersetGroupId,
    int? SupersetRestSeconds,
    int? TargetRpe);