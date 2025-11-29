using Microsoft.EntityFrameworkCore;
using trAInr.API.Data;
using trAInr.API.Models.Domain;
using trAInr.API.Models.DTOs;

namespace trAInr.API.Services;

public class UserService : IUserService
{
  private readonly TrainrDbContext _context;

  public UserService(TrainrDbContext context)
  {
    _context = context;
  }

  public async Task<UserResponse?> GetByIdAsync(Guid id)
  {
    var user = await _context.Users.FindAsync(id);
    return user is null ? null : MapToResponse(user);
  }

  public async Task<UserResponse?> GetByEmailAsync(string email)
  {
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    return user is null ? null : MapToResponse(user);
  }

  public async Task<IEnumerable<UserSummaryResponse>> GetAllAsync()
  {
    return await _context.Users
        .Select(u => new UserSummaryResponse(
            u.Id,
            u.Username,
            u.Email,
            $"{u.FirstName} {u.LastName}",
            u.FitnessLevel,
            u.PrimaryGoal))
        .ToListAsync();
  }

  public async Task<UserResponse> CreateAsync(CreateUserRequest request)
  {
    var user = new User
    {
      Id = Guid.NewGuid(),
      Username = request.Username,
      PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, BCrypt.Net.BCrypt.GenerateSalt(12)),
      Email = request.Email,
      FirstName = request.FirstName,
      LastName = request.LastName,
      DateOfBirth = request.DateOfBirth,
      FitnessLevel = request.FitnessLevel,
      PrimaryGoal = request.PrimaryGoal,
      WorkoutDaysPerWeek = request.WorkoutDaysPerWeek,
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow
    };

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    return MapToResponse(user);
  }

  public async Task<UserResponse?> UpdateAsync(Guid id, UpdateUserRequest request)
  {
    var user = await _context.Users.FindAsync(id);
    if (user is null) return null;

    user.FirstName = request.FirstName;
    user.LastName = request.LastName;
    user.FitnessLevel = request.FitnessLevel;
    user.PrimaryGoal = request.PrimaryGoal;
    user.WorkoutDaysPerWeek = request.WorkoutDaysPerWeek;
    user.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();
    return MapToResponse(user);
  }

  public async Task<bool> DeleteAsync(Guid id)
  {
    var user = await _context.Users.FindAsync(id);
    if (user is null) return false;

    _context.Users.Remove(user);
    await _context.SaveChangesAsync();
    return true;
  }

  public async Task<bool> ExistsAsync(Guid id)
  {
    return await _context.Users.AnyAsync(u => u.Id == id);
  }

  private static UserResponse MapToResponse(User user) =>
      new(
          user.Id,
          user.Username,
          user.Email,
          user.FirstName,
          user.LastName,
          user.DateOfBirth,
          user.FitnessLevel,
          user.PrimaryGoal,
          user.WorkoutDaysPerWeek,
          user.CreatedAt);
}

