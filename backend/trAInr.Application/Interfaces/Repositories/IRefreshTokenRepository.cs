using trAInr.Domain.Entities;

namespace trAInr.Application.Interfaces.Repositories;

/// <summary>
///     Repository for refresh token operations
/// </summary>
public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<IEnumerable<RefreshToken>> GetActiveTokensByAthleteIdAsync(Guid athleteId);
    Task AddAsync(RefreshToken refreshToken);
    Task UpdateAsync(RefreshToken refreshToken);
    Task RevokeAllForAthleteAsync(Guid athleteId);
    Task DeleteExpiredTokensAsync();
}
