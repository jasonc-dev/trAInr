using trAInr.API.Models.Domain;

namespace trAInr.API.Models.DTOs;

public record CreateUserRequest(
    string Email,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    FitnessLevel FitnessLevel,
    FitnessGoal PrimaryGoal,
    int WorkoutDaysPerWeek);

public record UpdateUserRequest(
    string FirstName,
    string LastName,
    FitnessLevel FitnessLevel,
    FitnessGoal PrimaryGoal,
    int WorkoutDaysPerWeek);

public record UserResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    FitnessLevel FitnessLevel,
    FitnessGoal PrimaryGoal,
    int WorkoutDaysPerWeek,
    DateTime CreatedAt);

public record UserSummaryResponse(
    Guid Id,
    string Email,
    string FullName,
    FitnessLevel FitnessLevel,
    FitnessGoal PrimaryGoal);

