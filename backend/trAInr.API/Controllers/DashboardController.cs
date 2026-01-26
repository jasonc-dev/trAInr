using Microsoft.AspNetCore.Mvc;
using trAInr.Application.DTOs;
using trAInr.Application.Interfaces.Services;

namespace trAInr.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController(IDashboardService dashboardService, IAthleteService athleteService)
    : ControllerBase
{
    /// <summary>
    ///     Get the dashboard for an athlete
    /// </summary>
    [HttpGet("athlete/{athleteId:guid}")]
    public async Task<ActionResult<DashboardResponse>> GetDashboard(Guid athleteId)
    {
        if (!await athleteService.ExistsAsync(athleteId)) return NotFound("Athlete not found");

        var dashboard = await dashboardService.GetDashboardAsync(athleteId);
        return Ok(dashboard);
    }

    /// <summary>
    ///     Get weekly progress for an assigned programme
    /// </summary>
    [HttpGet("programme/{programmeId:guid}/weekly-progress")]
    public async Task<ActionResult<IEnumerable<WeeklyMetrics>>> GetWeeklyProgress(Guid programmeId)
    {
        var metrics = await dashboardService.GetWeeklyProgressAsync(programmeId);
        return Ok(metrics);
    }

    /// <summary>
    ///     Get exercise metrics for an athlete
    /// </summary>
    [HttpGet("athlete/{athleteId:guid}/exercises")]
    public async Task<ActionResult<IEnumerable<ExerciseMetrics>>> GetExerciseMetrics(
        Guid athleteId,
        [FromQuery] int? exerciseId = null)
    {
        if (!await athleteService.ExistsAsync(athleteId)) return NotFound("Athlete not found");

        var metrics = await dashboardService.GetExerciseMetricsAsync(athleteId, exerciseId);
        return Ok(metrics);
    }


    /// <summary>
    ///     Get intensity trends for an assigned programme
    /// </summary>
    [HttpGet("programme/{programmeId:guid}/intensity-trends")]
    public async Task<ActionResult<IEnumerable<IntensityTrend>>> GetIntensityTrends(Guid programmeId)
    {
        var trends = await dashboardService.GetIntensityTrendsAsync(programmeId);
        return Ok(trends);
    }

    /// <summary>
    ///     Get volume comparison for an assigned programme
    /// </summary>
    [HttpGet("programme/{programmeId:guid}/volume-comparison")]
    public async Task<ActionResult<IEnumerable<VolumeComparison>>> GetVolumeComparison(Guid programmeId)
    {
        var comparison = await dashboardService.GetVolumeComparisonAsync(programmeId);
        return Ok(comparison);
    }
}