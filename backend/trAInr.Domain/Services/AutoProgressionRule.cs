namespace trAInr.Domain.Services;

/// <summary>
///     Domain service for applying auto-progression rules.
///     Encapsulates logic for determining load adjustments based on performance.
/// </summary>
public class AutoProgressionRule
{
    /// <summary>
    ///     Calculates the next load based on current performance.
    /// </summary>
    public decimal CalculateNextLoad(
        decimal currentLoad,
        int targetReps,
        int actualReps,
        int? rpe = null)
    {
        if (currentLoad <= 0)
            throw new ArgumentException("Current load must be positive", nameof(currentLoad));
        if (targetReps <= 0)
            throw new ArgumentException("Target reps must be positive", nameof(targetReps));
        if (actualReps <= 0)
            throw new ArgumentException("Actual reps must be positive", nameof(actualReps));

        // If RPE is provided and is low, we can increase load more aggressively
        var rpeMultiplier = rpe.HasValue ? GetRpeMultiplier(rpe.Value) : 1.0m;

        // If actual reps exceed target reps significantly, increase load
        if (actualReps >= targetReps + 2) return currentLoad * 1.05m * rpeMultiplier; // 5% increase

        // If actual reps meet or slightly exceed target, small increase
        if (actualReps >= targetReps) return currentLoad * 1.025m * rpeMultiplier; // 2.5% increase

        // If actual reps are close to target, maintain load
        if (actualReps >= targetReps - 1) return currentLoad;

        // If actual reps are below target, decrease load
        return currentLoad * 0.95m; // 5% decrease
    }

    private static decimal GetRpeMultiplier(int rpe)
    {
        // Lower RPE (easier) allows for more aggressive progression
        return rpe switch
        {
            <= 5 => 1.1m, // Very easy, can progress more
            <= 7 => 1.05m, // Moderate, standard progression
            <= 9 => 1.0m, // Hard, maintain or small increase
            _ => 0.95m // Maximum effort, consider deload
        };
    }
}