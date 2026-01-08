using trAInr.Domain.Aggregates;

namespace trAInr.Application.Interfaces.Services;

public interface IJwtTokenService
{
    string GenerateToken(Athlete athlete, DateTime expiresAt);
    Guid? ValidateToken(string token);
}
