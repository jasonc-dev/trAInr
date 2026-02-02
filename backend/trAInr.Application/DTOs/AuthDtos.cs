using trAInr.Domain.Aggregates;

namespace trAInr.Application.DTOs;

/// <summary>
///     Request payload for user login
/// </summary>
public record LoginRequest(
    string Username,
    string Password,
    string? DeviceInfo = null);

/// <summary>
///     Request payload for user registration
/// </summary>
public record RegisterRequest(
    string Username,
    string Password,
    string Email,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    TrainingLevel FitnessLevel,
    FitnessGoal PrimaryGoal,
    int WorkoutDaysPerWeek,
    string? DeviceInfo = null);

/// <summary>
///     Request payload for refreshing an access token
/// </summary>
public record RefreshTokenRequest(string RefreshToken);

/// <summary>
///     Request payload for revoking a refresh token
/// </summary>
public record RevokeTokenRequest(string RefreshToken);

/// <summary>
///     Response payload for successful authentication
/// </summary>
public record AuthResponse(
    Guid Id,
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt);

/// <summary>
///     Legacy response for backward compatibility (web client)
/// </summary>
public record LegacyAuthResponse(
    Guid Id,
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string Token,
    DateTime ExpiresAt);