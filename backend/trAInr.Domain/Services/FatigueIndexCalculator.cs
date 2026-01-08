using trAInr.Domain.ValueObjects;

namespace trAInr.Domain.Services;

/// <summary>
///     Domain service for calculating fatigue index based on training volume and intensity.
/// </summary>
public class FatigueIndexCalculator
{
    /// <summary>
    ///     Calculates fatigue index based on recent training volume and intensity.
    /// </summary>
    public FatigueIndex Calculate(
        decimal weeklyVolume,
        decimal averageIntensity,
        int trainingDays,
        int consecutiveWeeks)
    {
        if (weeklyVolume < 0)
            throw new ArgumentException("Weekly volume cannot be negative", nameof(weeklyVolume));
        if (averageIntensity < 0 || averageIntensity > 100)
            throw new ArgumentException("Average intensity must be between 0 and 100", nameof(averageIntensity));
        if (trainingDays < 0 || trainingDays > 7)
            throw new ArgumentException("Training days must be between 0 and 7", nameof(trainingDays));

        // Base fatigue from volume (normalized)
        var volumeFatigue = Math.Min(weeklyVolume / 10000m * 40m, 40m); // Max 40 points from volume

        // Intensity contribution (normalized)
        var intensityFatigue = averageIntensity / 100m * 30m; // Max 30 points from intensity

        // Consecutive weeks without deload (accumulated fatigue)
        var accumulatedFatigue = Math.Min(consecutiveWeeks * 5m, 30m); // Max 30 points from accumulation

        var totalFatigue = volumeFatigue + intensityFatigue + accumulatedFatigue;
        var fatigueIndex = Math.Min(totalFatigue, 100m); // Cap at 100

        return FatigueIndex.Create(fatigueIndex);
    }
}