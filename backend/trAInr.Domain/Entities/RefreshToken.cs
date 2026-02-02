namespace trAInr.Domain.Entities;

/// <summary>
///     Represents a refresh token for secure token rotation
/// </summary>
public class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid AthleteId { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? ReplacedByTokenId { get; private set; }
    public string? DeviceInfo { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsActive => !IsRevoked && !IsExpired;

    // For EF Core
    private RefreshToken() { }

    public RefreshToken(
        Guid id,
        Guid athleteId,
        string token,
        DateTime expiresAt,
        string? deviceInfo = null)
    {
        Id = id;
        AthleteId = athleteId;
        Token = token;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
        DeviceInfo = deviceInfo;
    }

    /// <summary>
    ///     Revoke this token, optionally specifying the replacement token
    /// </summary>
    public void Revoke(string? replacedByTokenId = null)
    {
        RevokedAt = DateTime.UtcNow;
        ReplacedByTokenId = replacedByTokenId;
    }
}
