namespace trAInr.Application.DTOs;

/// <summary>
///     Request for pulling changes from server
/// </summary>
public record SyncPullRequest
{
    /// <summary>
    ///     Last sync timestamp (ISO 8601). Returns all changes since this time.
    /// </summary>
    public DateTime? Since { get; init; }

    /// <summary>
    ///     Entity types to sync (e.g., "workout_days", "exercise_sets")
    /// </summary>
    public IEnumerable<string>? EntityTypes { get; init; }
}

/// <summary>
///     Response containing changes from server
/// </summary>
public record SyncPullResponse
{
    public DateTime ServerTimestamp { get; init; } = DateTime.UtcNow;
    public IEnumerable<SyncChange> Changes { get; init; } = [];
    public bool HasMore { get; init; }
    public string? ContinuationToken { get; init; }
}

/// <summary>
///     Request to push local changes to server
/// </summary>
public record SyncPushRequest
{
    public IEnumerable<SyncChange> Changes { get; init; } = [];
}

/// <summary>
///     Response from push operation
/// </summary>
public record SyncPushResponse
{
    public DateTime ServerTimestamp { get; init; } = DateTime.UtcNow;
    public IEnumerable<SyncChangeResult> Results { get; init; } = [];
}

/// <summary>
///     Represents a single change (create, update, delete)
/// </summary>
public record SyncChange
{
    /// <summary>
    ///     Unique idempotency key for this change (prevents duplicate processing)
    /// </summary>
    public string IdempotencyKey { get; init; } = string.Empty;

    /// <summary>
    ///     Entity type: "workout_days", "workout_exercises", "exercise_sets", "programme_weeks"
    /// </summary>
    public string EntityType { get; init; } = string.Empty;

    /// <summary>
    ///     Entity ID
    /// </summary>
    public string EntityId { get; init; } = string.Empty;

    /// <summary>
    ///     Operation: "create", "update", "delete"
    /// </summary>
    public string Operation { get; init; } = string.Empty;

    /// <summary>
    ///     JSON payload of the entity data
    /// </summary>
    public object? Data { get; init; }

    /// <summary>
    ///     Client timestamp when change was made
    /// </summary>
    public DateTime ClientTimestamp { get; init; }

    /// <summary>
    ///     Version of the entity (for conflict detection)
    /// </summary>
    public int? Version { get; init; }
}

/// <summary>
///     Result of processing a single sync change
/// </summary>
public record SyncChangeResult
{
    public string IdempotencyKey { get; init; } = string.Empty;
    public string EntityId { get; init; } = string.Empty;
    public SyncResultStatus Status { get; init; }
    public string? ErrorMessage { get; init; }
    public int? NewVersion { get; init; }
    public object? ServerData { get; init; }
}

/// <summary>
///     Status of a sync operation
/// </summary>
public enum SyncResultStatus
{
    Success,
    Conflict,
    NotFound,
    ValidationError,
    AlreadyProcessed
}

/// <summary>
///     Sync status for an entity (returned with entity responses)
/// </summary>
public record SyncMetadata
{
    public DateTime LastModifiedAt { get; init; }
    public int Version { get; init; }
    public bool IsDeleted { get; init; }
}
