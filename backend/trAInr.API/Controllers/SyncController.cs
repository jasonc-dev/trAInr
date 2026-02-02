using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using trAInr.API.Attributes;
using trAInr.Application.DTOs;
using trAInr.Application.Interfaces.Services;

namespace trAInr.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class SyncController(
    ISyncService syncService,
    ILogger<SyncController> logger) : ControllerBase
{
    /// <summary>
    ///     Pull changes from server since a given timestamp
    /// </summary>
    [HttpGet("pull")]
    public async Task<ActionResult<SyncPullResponse>> Pull([FromQuery] SyncPullRequest request)
    {
        var userId = HttpContext.Items["UserId"] as Guid?;
        if (userId is null) return Unauthorized();

        var response = await syncService.PullChangesAsync(userId.Value, request);
        return Ok(response);
    }

    /// <summary>
    ///     Push local changes to server with idempotency support
    /// </summary>
    [HttpPost("push")]
    public async Task<ActionResult<SyncPushResponse>> Push([FromBody] SyncPushRequest request)
    {
        var userId = HttpContext.Items["UserId"] as Guid?;
        if (userId is null) return Unauthorized();

        if (request.Changes == null || !request.Changes.Any())
            return BadRequest(new { message = "No changes provided" });

        var response = await syncService.PushChangesAsync(userId.Value, request);

        logger.LogInformation(
            "Processed {Count} sync changes for user {UserId}",
            request.Changes.Count(),
            userId.Value);

        return Ok(response);
    }

    /// <summary>
    ///     Get sync status for the current user
    /// </summary>
    [HttpGet("status")]
    public async Task<ActionResult<SyncStatusResponse>> GetStatus()
    {
        var userId = HttpContext.Items["UserId"] as Guid?;
        if (userId is null) return Unauthorized();

        var status = await syncService.GetSyncStatusAsync(userId.Value);
        return Ok(status);
    }
}

/// <summary>
///     Response containing sync status information
/// </summary>
public record SyncStatusResponse
{
    public DateTime ServerTimestamp { get; init; } = DateTime.UtcNow;
    public DateTime? LastSyncTimestamp { get; init; }
    public int PendingChangesCount { get; init; }
}
