using trAInr.Application.DTOs;

namespace trAInr.Application.Interfaces.Services;

/// <summary>
///     Service interface for authentication operations
/// </summary>
public interface IAuthService
{
    /// <summary>
    ///     Authenticate a user with username and password
    /// </summary>
    Task<AuthResponse?> LoginAsync(LoginRequest request);

    /// <summary>
    ///     Register a new user with hashed password
    /// </summary>
    Task<AuthResponse?> RegisterAsync(RegisterRequest request);

    /// <summary>
    ///     Refresh access token using a valid refresh token
    /// </summary>
    Task<AuthResponse?> RefreshTokenAsync(RefreshTokenRequest request);

    /// <summary>
    ///     Revoke a refresh token
    /// </summary>
    Task<bool> RevokeTokenAsync(RevokeTokenRequest request);

    /// <summary>
    ///     Revoke all refresh tokens for a user (logout from all devices)
    /// </summary>
    Task<bool> RevokeAllTokensAsync(Guid athleteId);

    /// <summary>
    ///     Validate a JWT token and return the user ID if valid
    /// </summary>
    Guid? ValidateToken(string token);

    /// <summary>
    ///     Check if a username already exists
    /// </summary>
    Task<bool> UsernameExistsAsync(string username);
}
