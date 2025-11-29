using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace trAInr.API.Attributes;

/// <summary>
/// Custom authorization attribute that checks for JWT authentication
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var userId = context.HttpContext.Items["UserId"];
        
        if (userId is null)
        {
            context.Result = new UnauthorizedObjectResult(new { message = "Unauthorized" });
        }
    }
}

/// <summary>
/// Attribute to allow anonymous access to endpoints
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AllowAnonymousAttribute : Attribute
{
}

