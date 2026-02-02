using trAInr.Domain.Entities;

namespace trAInr.Application.Interfaces.Repositories;

/// <summary>
///     Repository for idempotency record operations
/// </summary>
public interface IIdempotencyRepository
{
    Task<IdempotencyRecord?> GetByKeyAsync(string idempotencyKey, Guid athleteId);
    Task AddAsync(IdempotencyRecord record);
    Task<DateTime?> GetLastSyncTimeAsync(Guid athleteId);
    Task DeleteOldRecordsAsync(int daysToKeep = 30);
}
