using trAInr.API.Models.DTOs;

namespace trAInr.API.Services;

public interface IUserService
{
    Task<UserResponse?> GetByIdAsync(Guid id);
    Task<UserResponse?> GetByEmailAsync(string email);
    Task<IEnumerable<UserSummaryResponse>> GetAllAsync();
    Task<UserResponse> CreateAsync(CreateUserRequest request);
    Task<UserResponse?> UpdateAsync(Guid id, UpdateUserRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}

