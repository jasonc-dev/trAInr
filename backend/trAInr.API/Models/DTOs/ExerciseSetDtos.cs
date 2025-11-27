using trAInr.API.Models.Domain;

namespace trAInr.API.Models.DTOs;

public record CreateExerciseSetRequest(
    int SetNumber,
    int? Reps,
    decimal? Weight,
    int? DurationSeconds,
    decimal? Distance,
    Difficulty? Difficulty,
    Intensity? Intensity,
    bool IsWarmup,
    string? Notes);

public record UpdateExerciseSetRequest(
    int? Reps,
    decimal? Weight,
    int? DurationSeconds,
    decimal? Distance,
    Difficulty? Difficulty,
    Intensity? Intensity,
    bool IsCompleted,
    string? Notes);

public record ExerciseSetResponse(
    Guid Id,
    int SetNumber,
    int? Reps,
    decimal? Weight,
    int? DurationSeconds,
    decimal? Distance,
    Difficulty? Difficulty,
    Intensity? Intensity,
    bool IsCompleted,
    bool IsWarmup,
    string? Notes,
    DateTime? CompletedAt);

public record CompleteSetRequest(
    int? Reps,
    decimal? Weight,
    int? DurationSeconds,
    decimal? Distance,
    Difficulty? Difficulty,
    Intensity? Intensity,
    string? Notes);

