using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using trAInr.Application.DTOs;
using trAInr.Application.Interfaces;
using trAInr.Application.Interfaces.Repositories;
using trAInr.Application.Interfaces.Services;
using trAInr.Domain.Aggregates;
using trAInr.Domain.Entities;

namespace trAInr.Application.Services;

/// <summary>
///     Service for handling user authentication with password hashing, JWT tokens, and refresh tokens
/// </summary>
public class AuthService(
    IAthleteRepository athleteRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService,
    IUnitOfWork unitOfWork,
    ILogger<AuthService> logger)
    : IAuthService
{
    private const int AccessTokenExpirationMinutes = 15;
    private const int RefreshTokenExpirationDays = 30;

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
        return await CreateAuthResponseAsync(athlete, request.DeviceInfo);
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
        return await CreateAuthResponseAsync(athlete, request.DeviceInfo);
    }

    public async Task<AuthResponse?> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var refreshToken = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

        if (refreshToken is null)
        {
            logger.LogWarning("Refresh token not found");
            return null;
        }

        if (!refreshToken.IsActive)
        {
            logger.LogWarning("Refresh token is not active (expired or revoked) for athlete {AthleteId}", refreshToken.AthleteId);
            return null;
        }

        var athlete = await athleteRepository.GetByIdAsync(refreshToken.AthleteId);
        if (athlete is null)
        {
            logger.LogWarning("Athlete not found for refresh token");
            return null;
        }

        // Rotate the refresh token (revoke old one and create new one)
        var newRefreshToken = await RotateRefreshTokenAsync(refreshToken, refreshToken.DeviceInfo);

        logger.LogInformation("Token refreshed successfully for user {Username}", athlete.Username);
        return CreateAuthResponseFromTokens(athlete, newRefreshToken);
    }

    public async Task<bool> RevokeTokenAsync(RevokeTokenRequest request)
    {
        var refreshToken = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

        if (refreshToken is null)
        {
            logger.LogWarning("Revoke failed: Refresh token not found");
            return false;
        }

        if (!refreshToken.IsActive)
        {
            logger.LogWarning("Revoke failed: Token already revoked or expired");
            return false;
        }

        refreshToken.Revoke();
        await refreshTokenRepository.UpdateAsync(refreshToken);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Refresh token revoked for athlete {AthleteId}", refreshToken.AthleteId);
        return true;
    }

    public async Task<bool> RevokeAllTokensAsync(Guid athleteId)
    {
        await refreshTokenRepository.RevokeAllForAthleteAsync(athleteId);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("All refresh tokens revoked for athlete {AthleteId}", athleteId);
        return true;
    }

    public Guid? ValidateToken(string token)
    {
        return jwtTokenService.ValidateToken(token);
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await athleteRepository.ExistsByUsernameAsync(username);
    }

    private async Task<AuthResponse> CreateAuthResponseAsync(Athlete athlete, string? deviceInfo)
    {
        var refreshToken = await CreateRefreshTokenAsync(athlete.Id, deviceInfo);
        return CreateAuthResponseFromTokens(athlete, refreshToken);
    }

    private AuthResponse CreateAuthResponseFromTokens(Athlete athlete, RefreshToken refreshToken)
    {
        var accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(AccessTokenExpirationMinutes);
        var accessToken = jwtTokenService.GenerateToken(athlete, accessTokenExpiresAt);

        return new AuthResponse(
            athlete.Id,
            athlete.Username,
            athlete.Email,
            athlete.FirstName,
            athlete.LastName,
            accessToken,
            refreshToken.Token,
            accessTokenExpiresAt,
            refreshToken.ExpiresAt);
    }

    private async Task<RefreshToken> CreateRefreshTokenAsync(Guid athleteId, string? deviceInfo)
    {
        var tokenString = GenerateSecureToken();
        var expiresAt = DateTime.UtcNow.AddDays(RefreshTokenExpirationDays);

        var refreshToken = new RefreshToken(
            Guid.NewGuid(),
            athleteId,
            tokenString,
            expiresAt,
            deviceInfo);

        await refreshTokenRepository.AddAsync(refreshToken);
        await unitOfWork.SaveChangesAsync();

        return refreshToken;
    }

    private async Task<RefreshToken> RotateRefreshTokenAsync(RefreshToken oldToken, string? deviceInfo)
    {
        var newRefreshToken = await CreateRefreshTokenAsync(oldToken.AthleteId, deviceInfo);

        // Revoke the old token and link to new one
        oldToken.Revoke(newRefreshToken.Id.ToString());
        await refreshTokenRepository.UpdateAsync(oldToken);
        await unitOfWork.SaveChangesAsync();

        return newRefreshToken;
    }

    private static string GenerateSecureToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}