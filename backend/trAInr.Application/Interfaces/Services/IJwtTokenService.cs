using trAInr.Domain.Aggregates;

namespace trAInr.Application.Interfaces.Services;

public interface IJwtTokenService
{
    public string GenerateToken(Athlete athlete, DateTime expiresAt);
    public Guid? ValidateToken(string token);
}
