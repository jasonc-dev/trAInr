using Microsoft.Extensions.Logging;
using trAInr.Application.DTOs;
using trAInr.Application.Interfaces;
using trAInr.Application.Interfaces.Repositories;
using trAInr.Application.Interfaces.Services;
using trAInr.Domain.Aggregates;

namespace trAInr.Application.Services;

/// <summary>
///     Service for handling user authentication with password hashing and JWT tokens
/// </summary>
public class AuthService(
    IAthleteRepository athleteRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService,
    IUnitOfWork unitOfWork,
    ILogger<AuthService> logger)
    : IAuthService
{
    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var athlete = await athleteRepository.GetByUsernameAsync(request.Username);

        if (athlete is null)
        {
            logger.LogWarning("Login failed: User {Username} not found", request.Username);
            return null;
        }

        if (!passwordHasher.VerifyPassword(request.Password, athlete.PasswordHash))
        {
            logger.LogWarning("Login failed: Invalid password for user {Username}", request.Username);
            return null;
        }

        logger.LogInformation("User {Username} logged in successfully", request.Username);
        return CreateAuthResponse(athlete);
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        if (await athleteRepository.ExistsByUsernameAsync(request.Username))
        {
            logger.LogWarning("Registration failed: Username {Username} already exists", request.Username);
            return null;
        }

        if (await athleteRepository.ExistsByEmailAsync(request.Email))
        {
            logger.LogWarning("Registration failed: Email {Email} already exists", request.Email);
            return null;
        }

        var athlete = new Athlete(
            Guid.NewGuid(),
            request.Username,
            passwordHasher.HashPassword(request.Password),
            request.Email,
            request.FirstName,
            request.LastName,
            request.DateOfBirth,
            request.FitnessLevel,
            request.PrimaryGoal,
            request.WorkoutDaysPerWeek);

        await athleteRepository.AddAsync(athlete);
        await unitOfWork.SaveChangesAsync();

        athlete.ClearDomainEvents();

        logger.LogInformation("User {Username} registered successfully", request.Username);
        return CreateAuthResponse(athlete);
    }

    public Guid? ValidateToken(string token)
    {
        return jwtTokenService.ValidateToken(token);
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await athleteRepository.ExistsByUsernameAsync(username);
    }

    private AuthResponse CreateAuthResponse(Athlete athlete)
    {
        var expiresAt = DateTime.UtcNow.AddDays(7);
        var token = jwtTokenService.GenerateToken(athlete, expiresAt);

        return new AuthResponse(
            athlete.Id,
            athlete.Username,
            athlete.Email,
            athlete.FirstName,
            athlete.LastName,
            token,
            expiresAt);
    }
}