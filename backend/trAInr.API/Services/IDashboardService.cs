using trAInr.API.Models.DTOs;

namespace trAInr.API.Services;

public interface IDashboardService
{
    Task<DashboardResponse> GetDashboardAsync(Guid userId);
    Task<IEnumerable<WeeklyMetrics>> GetWeeklyProgressAsync(Guid programmeId);
    Task<IEnumerable<ExerciseMetrics>> GetExerciseMetricsAsync(Guid userId, Guid? exerciseId = null);
    Task<OverallStats> GetOverallStatsAsync(Guid userId);
    Task<IEnumerable<IntensityTrend>> GetIntensityTrendsAsync(Guid programmeId);
    Task<IEnumerable<VolumeComparison>> GetVolumeComparisonAsync(Guid programmeId);
}

