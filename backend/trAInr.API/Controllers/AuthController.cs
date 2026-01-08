using Microsoft.AspNetCore.Mvc;
using trAInr.Application.DTOs;
using trAInr.Application.Interfaces.Services;

namespace trAInr.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
}