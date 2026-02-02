using trAInr.Application.DTOs;

namespace trAInr.Application.Interfaces.Services;

/// <summary>
///     Service for handling offline sync operations
/// </summary>
public interface ISyncService
{
    /// <summary>
    ///     Pull changes from server since a given timestamp
    /// </summary>
    Task<SyncPullResponse> PullChangesAsync(Guid athleteId, SyncPullRequest request);

    /// <summary>
    ///     Push local changes to server with idempotency support
    /// </summary>
    Task<SyncPushResponse> PushChangesAsync(Guid athleteId, SyncPushRequest request);

    /// <summary>
    ///     Get sync status for a user
    /// </summary>
    Task<object> GetSyncStatusAsync(Guid athleteId);
}
