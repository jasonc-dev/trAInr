using trAInr.Domain.ValueObjects;

namespace trAInr.Domain.Services;

/// <summary>
///     Domain service for determining when and how to apply deloads.
/// </summary>
public class DeloadRule
{
    /// <summary>
    ///     Determines if a deload is needed based on fatigue and performance.
    /// </summary>
    public bool ShouldDeload(FatigueIndex fatigueIndex, int consecutiveWeeks, decimal? performanceDrop = null)
    {
        // High fatigue triggers deload
        if (fatigueIndex.Value >= 80)
            return true;

        // Extended training without deload
        if (consecutiveWeeks >= 4)
            return true;

        // Significant performance drop
        if (performanceDrop.HasValue && performanceDrop.Value >= 0.15m) // 15% drop
            return true;

        return false;
    }

    /// <summary>
    ///     Calculates the deload load (typically 60-70% of current load).
    /// </summary>
    public decimal CalculateDeloadLoad(decimal currentLoad, decimal deloadPercentage = 0.65m)
    {
        if (currentLoad <= 0)
            throw new ArgumentException("Current load must be positive", nameof(currentLoad));
        if (deloadPercentage < 0.5m || deloadPercentage > 0.8m)
            throw new ArgumentException("Deload percentage should be between 0.5 and 0.8", nameof(deloadPercentage));

        return currentLoad * deloadPercentage;
    }
}