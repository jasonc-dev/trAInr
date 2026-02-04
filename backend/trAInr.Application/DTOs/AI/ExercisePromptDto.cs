namespace trAInr.Application.DTOs.AI;

public record ExercisePromptDto(
    int Id,
    string Name,
    string MovementPattern,
    string ExerciseType,
    string[] Equipment,
    string[] PrimaryMuscles,
    string[] SecondaryMuscles,
    string Difficulty,
    string SpinalLoad,
    string SetupComplexity,
    string TrackingMode,
    string? DefaultRepRange,
    string[] Tags,
    string? ShortCue
);
