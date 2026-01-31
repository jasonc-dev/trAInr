using Microsoft.Extensions.Logging;
using trAInr.Application.DTOs;
using trAInr.Application.Interfaces;
using trAInr.Application.Interfaces.Repositories;
using trAInr.Application.Interfaces.Services;
using trAInr.Domain.Aggregates;
using trAInr.Domain.ValueObjects;

namespace trAInr.Application.Services;

public class AthleteService(
    IAthleteRepository athleteRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork)
    : IAthleteService
{
    public async Task<UserResponse?> GetByIdAsync(Guid id)
    {
        var athlete = await athleteRepository.GetByIdAsync(id);
        return athlete is null ? null : MapToResponse(athlete);
    }

    public async Task<UserResponse?> GetByEmailAsync(string email)
    {
        var athlete = await athleteRepository.GetByEmailAsync(email);
        return athlete is null ? null : MapToResponse(athlete);
    }

    public async Task<IEnumerable<UserSummaryResponse>> GetAllAsync()
    {
        var athletes = await athleteRepository.GetAllAsync();
        return athletes.Select(a => new UserSummaryResponse(
            a.Id,
            a.Username,
            a.Email,
            $"{a.FirstName} {a.LastName}",
            a.Level,
            a.PrimaryGoal));
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request)
    {
        if (await athleteRepository.ExistsByEmailAsync(request.Email))
            throw new InvalidOperationException("A user with this email already exists");

        if (await athleteRepository.ExistsByUsernameAsync(request.Username))
            throw new InvalidOperationException("A user with this username already exists");

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

        // Dispatch domain events if needed
        athlete.ClearDomainEvents();

        return MapToResponse(athlete);
    }

    public async Task<UserResponse?> UpdateAsync(Guid id, UpdateUserRequest request)
    {
        var athlete = await athleteRepository.GetByIdAsync(id);
        if (athlete is null) return null;

        // Use domain methods where applicable
        if (athlete.Level != request.FitnessLevel)
        {
            athlete.AdjustTrainingLevel(request.FitnessLevel, "User profile update");
        }

        // Update profile using domain method
        athlete.UpdateProfile(
            request.FirstName,
            request.LastName,
            request.PrimaryGoal,
            request.WorkoutDaysPerWeek);

        // Update equipment preferences if provided
        // Note: The domain model doesn't support removing equipment, so we need to work with reflection
        // or accept that equipment can only be added. For proper implementation, consider adding
        // a domain method to clear/replace equipment preferences.
        if (request.EquipmentPreferences is not null)
        {
            // For now, we'll use reflection to clear the equipment list
            var equipmentField = typeof(Athlete).GetField("_equipmentPreferences", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (equipmentField?.GetValue(athlete) is List<EquipmentRequirement> equipmentList)
            {
                equipmentList.Clear();
            }

            // Add new equipment preferences
            foreach (var equipmentName in request.EquipmentPreferences)
            {
                if (!string.IsNullOrWhiteSpace(equipmentName))
                {
                    athlete.AddEquipmentPreference(EquipmentRequirement.Create(equipmentName, true));
                }
            }
        }

        await athleteRepository.UpdateAsync(athlete);
        await unitOfWork.SaveChangesAsync();

        athlete.ClearDomainEvents();
        return MapToResponse(athlete);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var athlete = await athleteRepository.GetByIdAsync(id);
        if (athlete is null) return false;

        await athleteRepository.DeleteAsync(athlete);
        await unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await athleteRepository.ExistsAsync(id);
    }

    private static UserResponse MapToResponse(Athlete athlete)
    {
        return new UserResponse(
            athlete.Id,
            athlete.Username,
            athlete.Email,
            athlete.FirstName,
            athlete.LastName,
            athlete.DateOfBirth,
            athlete.Level,
            athlete.PrimaryGoal,
            athlete.WorkoutDaysPerWeek,
            athlete.EquipmentPreferences.Select(e => e.Name).ToList(),
            athlete.CreatedAt);
    }
}