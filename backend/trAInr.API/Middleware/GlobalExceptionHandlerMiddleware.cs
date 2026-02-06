using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace trAInr.API.Middleware;

/// <summary>
///     Global exception handling middleware
///     Catches all unhandled exceptions and returns consistent error responses
/// </summary>
public class GlobalExceptionHandlerMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionHandlerMiddleware> logger,
    IWebHostEnvironment environment)
{
    private static readonly JsonSerializerOptions ProductionJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private static readonly JsonSerializerOptions DevelopmentJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public async Task InvokeAsync(HttpContext context)
    {
        // Generate a request ID for tracking
        var requestId = context.TraceIdentifier;
        context.Response.Headers["X-Request-Id"] = requestId;

        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Request {RequestId}: An unhandled exception occurred: {Message}", requestId, ex.Message);
            await HandleExceptionAsync(context, ex, requestId);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, string requestId)
    {
        context.Response.ContentType = "application/json";
        var response = context.Response;

        var (statusCode, errorCode) = GetStatusCodeAndErrorCode(exception);

        var errorResponse = new ErrorResponse
        {
            Code = errorCode,
            Message = GetErrorMessage(exception),
            RequestId = requestId,
            Timestamp = DateTime.UtcNow,
            Retryable = IsRetryable(exception)
        };

        // Include detailed error information in development environment
        if (environment.IsDevelopment())
        {
            errorResponse.Details = exception.ToString();
            errorResponse.StackTrace = exception.StackTrace;
        }

        // Add validation errors if it's a validation exception
        if (exception is ArgumentException or ArgumentNullException)
        {
            errorResponse.Errors = new Dictionary<string, string[]>
            {
                { "validation", new[] { exception.Message } }
            };
        }

        response.StatusCode = (int)statusCode;

        var options = environment.IsDevelopment() ? DevelopmentJsonOptions : ProductionJsonOptions;
        var jsonResponse = JsonSerializer.Serialize(errorResponse, options);
        await response.WriteAsync(jsonResponse);
    }

    private static (HttpStatusCode statusCode, string errorCode) GetStatusCodeAndErrorCode(Exception exception)
    {
        return exception switch
        {
            ArgumentException or ArgumentNullException => (HttpStatusCode.BadRequest, "VALIDATION_ERROR"),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "UNAUTHORIZED"),
            KeyNotFoundException => (HttpStatusCode.NotFound, "NOT_FOUND"),
            InvalidOperationException => (HttpStatusCode.BadRequest, "INVALID_OPERATION"),
            DbUpdateException => (HttpStatusCode.Conflict, "DATABASE_CONFLICT"),
            TimeoutException => (HttpStatusCode.RequestTimeout, "TIMEOUT"),
            NotSupportedException => (HttpStatusCode.NotImplemented, "NOT_SUPPORTED"),
            _ => (HttpStatusCode.InternalServerError, "INTERNAL_ERROR")
        };
    }

    private static string GetErrorMessage(Exception exception)
    {
        return exception switch
        {
            ArgumentException or ArgumentNullException => "Invalid request. Please check your input and try again.",
            UnauthorizedAccessException => "You are not authorized to perform this action.",
            KeyNotFoundException => "The requested resource was not found.",
            InvalidOperationException => exception.Message,
            DbUpdateException => "A database error occurred. Please try again later.",
            TimeoutException => "The request timed out. Please try again.",
            NotSupportedException => "This operation is not supported.",
            _ => "An unexpected error occurred. Please try again later."
        };
    }

    private static bool IsRetryable(Exception exception)
    {
        return exception switch
        {
            TimeoutException => true,
            DbUpdateException => true,
            _ when exception.Message.Contains("temporarily unavailable", StringComparison.OrdinalIgnoreCase) => true,
            _ => false
        };
    }
}

/// <summary>
///     Error response model for consistent API error responses (mobile-friendly)
/// </summary>
public class ErrorResponse
{
    /// <summary>
    ///     Machine-readable error code (e.g., VALIDATION_ERROR, NOT_FOUND)
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    ///     Human-readable error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    ///     Unique request ID for tracking and debugging
    /// </summary>
    public string RequestId { get; set; } = string.Empty;

    /// <summary>
    ///     Timestamp when the error occurred
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    ///     Whether the operation can be retried
    /// </summary>
    public bool Retryable { get; set; }

    /// <summary>
    ///     Detailed error information (development only)
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    ///     Stack trace (development only)
    /// </summary>
    public string? StackTrace { get; set; }

    /// <summary>
    ///     Field-level validation errors
    /// </summary>
    public Dictionary<string, string[]>? Errors { get; set; }
}

/// <summary>
///     Extension methods for global exception handler middleware
/// </summary>
public static class GlobalExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}

