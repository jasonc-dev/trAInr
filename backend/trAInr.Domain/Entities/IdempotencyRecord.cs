namespace trAInr.Domain.Entities;

/// <summary>
///     Tracks processed idempotency keys to prevent duplicate operations
/// </summary>
public class IdempotencyRecord
{
    public Guid Id { get; private set; }
    public string IdempotencyKey { get; private set; } = string.Empty;
    public Guid AthleteId { get; private set; }
    public string EntityType { get; private set; } = string.Empty;
    public string EntityId { get; private set; } = string.Empty;
    public string Operation { get; private set; } = string.Empty;
    public DateTime ProcessedAt { get; private set; }
    public string? ResponseData { get; private set; }

    // For EF Core
    private IdempotencyRecord() { }

    public IdempotencyRecord(
        Guid id,
        string idempotencyKey,
        Guid athleteId,
        string entityType,
        string entityId,
        string operation,
        string? responseData = null)
    {
        Id = id;
        IdempotencyKey = idempotencyKey;
        AthleteId = athleteId;
        EntityType = entityType;
        EntityId = entityId;
        Operation = operation;
        ProcessedAt = DateTime.UtcNow;
        ResponseData = responseData;
    }
}
