using trAInr.Domain.Entities;

namespace trAInr.Application.DTOs;

public record CreateExerciseSetRequest(
    int SetNumber,
    int? Reps,
    decimal? Weight,
    int? DurationSeconds,
    decimal? Distance,
    Difficulty? Difficulty,
    Intensity? Intensity,
    bool IsWarmup,
    SetType SetType,
    decimal? DropPercentage,
    string? Notes);

public record UpdateExerciseSetRequest(
    int? Reps,
    decimal? Weight,
    int? DurationSeconds,
    decimal? Distance,
    Difficulty? Difficulty,
    Intensity? Intensity,
    bool IsCompleted,
    SetType? SetType,
    decimal? DropPercentage,
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
    SetType SetType,
    decimal? DropPercentage,
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

/// <summary>
/// Request to create a sequence of drop sets based on a template
/// </summary>
public record CreateDropSetRequest(
    decimal StartingWeight,
    int StartingReps,
    int NumberOfDrops,
    decimal DropPercentage,
    int RepsAdjustment);
