using trAInr.API.Models.DTOs;

namespace trAInr.API.Services;

/// <summary>
/// Service interface for authentication operations
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticate a user with username and password
    /// </summary>
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    
    /// <summary>
    /// Register a new user with hashed password
    /// </summary>
    Task<AuthResponse?> RegisterAsync(RegisterRequest request);
    
    /// <summary>
    /// Validate a JWT token and return the user ID if valid
    /// </summary>
    Guid? ValidateToken(string token);
    
    /// <summary>
    /// Check if a username already exists
    /// </summary>
    Task<bool> UsernameExistsAsync(string username);
}

