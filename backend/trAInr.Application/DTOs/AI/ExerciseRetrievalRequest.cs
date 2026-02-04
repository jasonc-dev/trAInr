using trAInr.Domain.Entities;

namespace trAInr.Application.DTOs.AI;

public record ExerciseRetrievalRequest(
    string[] AvailableEquipment,
    LevelOfDifficulty MaxDifficulty,
    string[]? ExcludedTags = null,
    string[]? RequiredTags = null,
    string? Goal = null,
    string? TargetFocus = null,
    int Limit = 120,
    int PerMovementPatternLimit = 20
);
