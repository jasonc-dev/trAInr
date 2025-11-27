using Microsoft.AspNetCore.Mvc;
using trAInr.API.Models.DTOs;
using trAInr.API.Services;

namespace trAInr.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly IUserService _userService;

    public DashboardController(IDashboardService dashboardService, IUserService userService)
    {
        _dashboardService = dashboardService;
        _userService = userService;
    }

    /// <summary>
    /// Get the dashboard for a user
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<DashboardResponse>> GetDashboard(Guid userId)
    {
        if (!await _userService.ExistsAsync(userId))
        {
            return NotFound("User not found");
        }

        var dashboard = await _dashboardService.GetDashboardAsync(userId);
        return Ok(dashboard);
    }

    /// <summary>
    /// Get weekly progress for a programme
    /// </summary>
    [HttpGet("programme/{programmeId:guid}/weekly-progress")]
    public async Task<ActionResult<IEnumerable<WeeklyMetrics>>> GetWeeklyProgress(Guid programmeId)
    {
        var metrics = await _dashboardService.GetWeeklyProgressAsync(programmeId);
        return Ok(metrics);
    }

    /// <summary>
    /// Get exercise metrics for a user
    /// </summary>
    [HttpGet("user/{userId:guid}/exercises")]
    public async Task<ActionResult<IEnumerable<ExerciseMetrics>>> GetExerciseMetrics(
        Guid userId,
        [FromQuery] Guid? exerciseId = null)
    {
        if (!await _userService.ExistsAsync(userId))
        {
            return NotFound("User not found");
        }

        var metrics = await _dashboardService.GetExerciseMetricsAsync(userId, exerciseId);
        return Ok(metrics);
    }

    /// <summary>
    /// Get overall stats for a user
    /// </summary>
    [HttpGet("user/{userId:guid}/stats")]
    public async Task<ActionResult<OverallStats>> GetOverallStats(Guid userId)
    {
        if (!await _userService.ExistsAsync(userId))
        {
            return NotFound("User not found");
        }

        var stats = await _dashboardService.GetOverallStatsAsync(userId);
        return Ok(stats);
    }

    /// <summary>
    /// Get intensity trends for a programme
    /// </summary>
    [HttpGet("programme/{programmeId:guid}/intensity-trends")]
    public async Task<ActionResult<IEnumerable<IntensityTrend>>> GetIntensityTrends(Guid programmeId)
    {
        var trends = await _dashboardService.GetIntensityTrendsAsync(programmeId);
        return Ok(trends);
    }

    /// <summary>
    /// Get volume comparison for a programme
    /// </summary>
    [HttpGet("programme/{programmeId:guid}/volume-comparison")]
    public async Task<ActionResult<IEnumerable<VolumeComparison>>> GetVolumeComparison(Guid programmeId)
    {
        var comparison = await _dashboardService.GetVolumeComparisonAsync(programmeId);
        return Ok(comparison);
    }
}

