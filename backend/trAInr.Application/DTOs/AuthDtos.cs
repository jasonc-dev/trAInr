using trAInr.Domain.Aggregates;

namespace trAInr.Application.DTOs;

/// <summary>
///     Request payload for user login
/// </summary>
public record LoginRequest(
    string Username,
    string Password);

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
    int WorkoutDaysPerWeek);

/// <summary>
///     Response payload for successful authentication
/// </summary>
public record AuthResponse(
    Guid Id,
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string Token,
    DateTime ExpiresAt);