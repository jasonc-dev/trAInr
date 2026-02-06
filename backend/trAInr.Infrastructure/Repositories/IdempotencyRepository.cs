using Microsoft.EntityFrameworkCore;
using trAInr.Application.Interfaces.Repositories;
using trAInr.Domain.Entities;
using trAInr.Infrastructure.Data;

namespace trAInr.Infrastructure.Repositories;

/// <summary>
///     Repository for idempotency record operations
/// </summary>
public class IdempotencyRepository(TrainrDbContext context) : IIdempotencyRepository
{
    public async Task<IdempotencyRecord?> GetByKeyAsync(string idempotencyKey, Guid athleteId)
    {
        return await context.IdempotencyRecords
            .FirstOrDefaultAsync(r => r.IdempotencyKey == idempotencyKey && r.AthleteId == athleteId);
    }

    public async Task AddAsync(IdempotencyRecord record)
    {
        await context.IdempotencyRecords.AddAsync(record);
    }

    public async Task<DateTime?> GetLastSyncTimeAsync(Guid athleteId)
    {
        return await context.IdempotencyRecords
            .Where(r => r.AthleteId == athleteId)
            .OrderByDescending(r => r.ProcessedAt)
            .Select(r => (DateTime?)r.ProcessedAt)
            .FirstOrDefaultAsync();
    }

    public async Task DeleteOldRecordsAsync(int daysToKeep = 30)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);
        var oldRecords = await context.IdempotencyRecords
            .Where(r => r.ProcessedAt < cutoffDate)
            .ToListAsync();

        context.IdempotencyRecords.RemoveRange(oldRecords);
    }
}
