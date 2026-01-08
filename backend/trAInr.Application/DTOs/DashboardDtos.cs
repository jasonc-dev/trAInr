using trAInr.Domain.Entities;

namespace trAInr.Application.DTOs;

/// <summary>
///     Dashboard metrics for programme tracking
/// </summary>
public record DashboardResponse(
    ProgrammeSummaryResponse? ActiveProgramme,
    WeeklyMetrics CurrentWeekMetrics,
    IEnumerable<WeeklyMetrics> WeeklyProgress,
    IEnumerable<ExerciseMetrics> TopExercises,
    OverallStats OverallStats);

/// <summary>
///     Metrics for a single week
/// </summary>
public record WeeklyMetrics(
    int WeekNumber,
    decimal TotalVolume,
    decimal AverageIntensity,
    int WorkoutsCompleted,
    int WorkoutsPlanned,
    int TotalSetsCompleted,
    int TotalReps,
    TimeSpan TotalDuration);

/// <summary>
///     Metrics for a specific exercise across the programme
/// </summary>
public record ExerciseMetrics(
    Guid ExerciseId,
    string ExerciseName,
    ExerciseType ExerciseType,
    decimal TotalVolume,
    decimal MaxWeight,
    int TotalSets,
    int TotalReps,
    decimal AverageReps,
    decimal AverageWeight,
    IEnumerable<ExerciseProgressPoint> ProgressPoints);

/// <summary>
///     Single data point for exercise progress tracking
/// </summary>
public record ExerciseProgressPoint(
    DateTime Date,
    int WeekNumber,
    decimal Volume,
    decimal MaxWeight,
    int TotalReps,
    decimal AverageIntensity);

/// <summary>
///     Overall statistics for a user's training history
/// </summary>
public record OverallStats(
    int TotalWorkoutsCompleted,
    int TotalSetsCompleted,
    int TotalRepsPerformed,
    decimal TotalVolumeLifted,
    TimeSpan TotalTrainingTime,
    int CurrentStreak,
    int LongestStreak);

/// <summary>
///     Intensity trend data for visualisation
/// </summary>
public record IntensityTrend(
    int WeekNumber,
    decimal AverageIntensity,
    decimal AverageDifficulty,
    string Trend);

/// <summary>
///     Volume comparison between weeks
/// </summary>
public record VolumeComparison(
    int WeekNumber,
    decimal Volume,
    decimal PercentageChange);
