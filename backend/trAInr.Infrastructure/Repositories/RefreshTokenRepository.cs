using Microsoft.EntityFrameworkCore;
using trAInr.Application.Interfaces.Repositories;
using trAInr.Domain.Entities;
using trAInr.Infrastructure.Data;

namespace trAInr.Infrastructure.Repositories;

/// <summary>
///     Repository for refresh token operations
/// </summary>
public class RefreshTokenRepository(TrainrDbContext context) : IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task<IEnumerable<RefreshToken>> GetActiveTokensByAthleteIdAsync(Guid athleteId)
    {
        return await context.RefreshTokens
            .Where(rt => rt.AthleteId == athleteId && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task AddAsync(RefreshToken refreshToken)
    {
        await context.RefreshTokens.AddAsync(refreshToken);
    }

    public Task UpdateAsync(RefreshToken refreshToken)
    {
        context.RefreshTokens.Update(refreshToken);
        return Task.CompletedTask;
    }

    public async Task RevokeAllForAthleteAsync(Guid athleteId)
    {
        var tokens = await context.RefreshTokens
            .Where(rt => rt.AthleteId == athleteId && rt.RevokedAt == null)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.Revoke();
        }
    }

    public async Task DeleteExpiredTokensAsync()
    {
        var expiredTokens = await context.RefreshTokens
            .Where(rt => rt.ExpiresAt < DateTime.UtcNow.AddDays(-7)) // Keep expired tokens for 7 days for audit
            .ToListAsync();

        context.RefreshTokens.RemoveRange(expiredTokens);
    }
}
