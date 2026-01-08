using trAInr.Application.Interfaces.Services;

namespace trAInr.API.Middleware;

/// <summary>
///     Middleware for JWT authentication
///     Validates the Authorization header and attaches user context
/// </summary>
public class JwtAuthenticationMiddleware(
    RequestDelegate next,
    ILogger<JwtAuthenticationMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, IAuthService authService)
    {
        var token = ExtractTokenFromHeader(context);

        if (!string.IsNullOrEmpty(token))
        {
            var userId = authService.ValidateToken(token);

            if (userId.HasValue)
            {
                context.Items["UserId"] = userId.Value;
                logger.LogDebug("User {UserId} authenticated via JWT", userId.Value);
            }
            else
            {
                logger.LogWarning("Invalid JWT token received");
            }
        }

        await next(context);
    }

    private static string? ExtractTokenFromHeader(HttpContext context)
    {
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();

        if (string.IsNullOrEmpty(authHeader))
            return null;

        if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return authHeader["Bearer ".Length..].Trim();

        return null;
    }
}

/// <summary>
///     Extension methods for JWT authentication middleware
/// </summary>
public static class JwtAuthenticationMiddlewareExtensions
{
    public static IApplicationBuilder UseJwtAuthentication(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<JwtAuthenticationMiddleware>();
    }
}