using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using trAInr.Application.DTOs;
using trAInr.Application.Interfaces.Services;

namespace trAInr.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly ILogger<AuthController> _logger = logger;

    /// <summary>
    ///     Authenticate user with username and password
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { message = "Username and password are required" });

        var result = await _authService.LoginAsync(request);

        if (result is null) return Unauthorized(new { message = "Invalid username or password" });

        _logger.LogInformation("User {Username} logged in successfully", request.Username);
        return Ok(result);
    }

    /// <summary>
    ///     Register a new user account
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { message = "Username and password are required" });

        if (request.Password.Length < 6) return BadRequest(new { message = "Password must be at least 6 characters" });

        if (await _authService.UsernameExistsAsync(request.Username))
            return Conflict(new { message = "Username already exists" });

        var result = await _authService.RegisterAsync(request);

        if (result is null) return BadRequest(new { message = "Registration failed. Email may already be in use." });

        _logger.LogInformation("New user {Username} registered successfully", request.Username);
        return CreatedAtAction(nameof(Login), result);
    }

    /// <summary>
    ///     Check if a username is available
    /// </summary>
    [HttpGet("check-username/{username}")]
    public async Task<ActionResult<object>> CheckUsername(string username)
    {
        var exists = await _authService.UsernameExistsAsync(username);
        return Ok(new { available = !exists });
    }

    /// <summary>
    ///     Refresh access token using a valid refresh token
    /// </summary>
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return BadRequest(new { message = "Refresh token is required" });

        var result = await _authService.RefreshTokenAsync(request);

        if (result is null)
            return Unauthorized(new { message = "Invalid or expired refresh token" });

        _logger.LogInformation("Token refreshed successfully for user {UserId}", result.Id);
        return Ok(result);
    }

    /// <summary>
    ///     Revoke a refresh token (logout from specific device)
    /// </summary>
    [HttpPost("revoke")]
    public async Task<ActionResult> RevokeToken([FromBody] RevokeTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return BadRequest(new { message = "Refresh token is required" });

        var result = await _authService.RevokeTokenAsync(request);

        if (!result)
            return NotFound(new { message = "Token not found or already revoked" });

        return Ok(new { message = "Token revoked successfully" });
    }

    /// <summary>
    ///     Revoke all refresh tokens for the current user (logout from all devices)
    /// </summary>
    [HttpPost("revoke-all")]
    [trAInr.API.Attributes.Authorize]
    public async Task<ActionResult> RevokeAllTokens()
    {
        var userId = HttpContext.Items["UserId"] as Guid?;
        if (userId is null)
            return Unauthorized();

        await _authService.RevokeAllTokensAsync(userId.Value);

        _logger.LogInformation("All tokens revoked for user {UserId}", userId.Value);
        return Ok(new { message = "All tokens revoked successfully" });
    }
}