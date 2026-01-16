using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using trAInr.Application.Interfaces.Services;
using trAInr.Domain.Aggregates;

namespace trAInr.Infrastructure.Services;

public class JwtTokenService(IConfiguration configuration, ILogger<JwtTokenService> logger)
    : IJwtTokenService
{
    public string GenerateToken(Athlete athlete, DateTime expiresAt)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(GetJwtSecret());

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, athlete.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, athlete.Username),
            new(ClaimTypes.Email, athlete.Email)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt,
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public Guid? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(GetJwtSecret());

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = configuration["Jwt:Audience"],
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == "sub").Value);

            return userId;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Token validation failed");
            return null;
        }
    }

    private string GetJwtSecret()
    {
        var secret = configuration["Jwt:Secret"];
        if (string.IsNullOrEmpty(secret))
        {
            throw new InvalidOperationException("JWT Secret is not configured or is empty");
        }

        // HMAC-SHA256 requires at least 256 bits (32 bytes) key length
        if (Encoding.UTF8.GetBytes(secret).Length < 32)
        {
            throw new InvalidOperationException("JWT Secret must be at least 32 characters long for HMAC-SHA256");
        }

        return secret;
    }
}