using trAInr.Domain.Aggregates;

namespace trAInr.Domain.Services;

/// <summary>
///     Domain service for calculating training volume and load metrics.
/// </summary>
public class VolumeLoadCalculator
{
    /// <summary>
    ///     Calculates total volume (weight × reps) for a workout session.
    /// </summary>
    public decimal CalculateTotalVolume(IEnumerable<CompletedSet> sets)
    {
        return sets
            .Where(s => s.Weight.HasValue && s.Reps.HasValue)
            .Sum(s => s.Weight!.Value * s.Reps!.Value);
    }

    /// <summary>
    ///     Calculates total volume for a specific exercise.
    /// </summary>
    public decimal CalculateExerciseVolume(IEnumerable<CompletedSet> sets, Guid exerciseInstanceId)
    {
        return sets
            .Where(s => s.ExerciseInstanceId == exerciseInstanceId && s.Weight.HasValue && s.Reps.HasValue)
            .Sum(s => s.Weight!.Value * s.Reps!.Value);
    }

    /// <summary>
    ///     Calculates average intensity (percentage of estimated 1RM).
    /// </summary>
    public decimal CalculateAverageIntensity(IEnumerable<CompletedSet> sets)
    {
        var intensitySets = sets
            .Where(s => s.Weight.HasValue && s.Reps.HasValue && s.Reps.Value > 0)
            .ToList();

        if (!intensitySets.Any())
            return 0;

        // Estimate 1RM using Epley formula: 1RM = weight × (1 + reps/30)
        var estimated1RMs = intensitySets.Select(s =>
        {
            var estimated1RM = s.Weight!.Value * (1 + s.Reps!.Value / 30m);
            return s.Weight.Value / estimated1RM * 100m; // Intensity as percentage of 1RM
        });

        return estimated1RMs.Average();
    }

    /// <summary>
    ///     Calculates weekly volume from multiple sessions.
    /// </summary>
    public decimal CalculateWeeklyVolume(IEnumerable<WorkoutSession> sessions)
    {
        return sessions
            .Where(s => s.IsCompleted)
            .SelectMany(s => s.ExerciseInstances)
            .SelectMany(e => e.CompletedSets)
            .Where(s => s.Weight.HasValue && s.Reps.HasValue)
            .Sum(s => s.Weight!.Value * s.Reps!.Value);
    }
}