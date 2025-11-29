namespace trAInr.API.Models.DTOs;

/// <summary>
/// Request payload for user login
/// </summary>
public record LoginRequest(
    string Username,
    string Password);

/// <summary>
/// Request payload for user registration
/// </summary>
public record RegisterRequest(
    string Username,
    string Password,
    string Email,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    Domain.FitnessLevel FitnessLevel,
    Domain.FitnessGoal PrimaryGoal,
    int WorkoutDaysPerWeek);

/// <summary>
/// Response payload for successful authentication
/// </summary>
public record AuthResponse(
    Guid Id,
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string Token,
    DateTime ExpiresAt);

