using trAInr.Domain.Aggregates;

namespace trAInr.Application.DTOs;

public record CreateUserRequest(
    string Username,
    string Password,
    string Email,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    TrainingLevel FitnessLevel,
    FitnessGoal PrimaryGoal,
    int WorkoutDaysPerWeek);

public record UpdateUserRequest(
    string FirstName,
    string LastName,
    TrainingLevel FitnessLevel,
    FitnessGoal PrimaryGoal,
    int WorkoutDaysPerWeek);

public record UserResponse(
    Guid Id,
    string Username,
    string Email,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    TrainingLevel FitnessLevel,
    FitnessGoal PrimaryGoal,
    int WorkoutDaysPerWeek,
    DateTime CreatedAt);

public record UserSummaryResponse(
    Guid Id,
    string Username,
    string Email,
    string FullName,
    TrainingLevel FitnessLevel,
    FitnessGoal PrimaryGoal);
