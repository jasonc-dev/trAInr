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
    try
    {
      await next(context);
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
      await HandleExceptionAsync(context, ex);
    }
  }

  private async Task HandleExceptionAsync(HttpContext context, Exception exception)
  {
    context.Response.ContentType = "application/json";
    var response = context.Response;

    var errorResponse = new ErrorResponse
    {
      StatusCode = GetStatusCode(exception),
      Message = GetErrorMessage(exception),
      Timestamp = DateTime.UtcNow
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

    response.StatusCode = (int)errorResponse.StatusCode;

    var options = environment.IsDevelopment() ? DevelopmentJsonOptions : ProductionJsonOptions;
    var jsonResponse = JsonSerializer.Serialize(errorResponse, options);
    await response.WriteAsync(jsonResponse);
  }

  private static HttpStatusCode GetStatusCode(Exception exception)
  {
    return exception switch
    {
      ArgumentException or ArgumentNullException => HttpStatusCode.BadRequest,
      UnauthorizedAccessException => HttpStatusCode.Unauthorized,
      KeyNotFoundException or InvalidOperationException => HttpStatusCode.NotFound,
      DbUpdateException => HttpStatusCode.Conflict,
      TimeoutException => HttpStatusCode.RequestTimeout,
      NotSupportedException => HttpStatusCode.NotImplemented,
      _ => HttpStatusCode.InternalServerError
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
      DbUpdateException dbEx => "A database error occurred. Please try again later.",
      TimeoutException => "The request timed out. Please try again.",
      NotSupportedException => "This operation is not supported.",
      _ => "An unexpected error occurred. Please try again later."
    };
  }
}

/// <summary>
///     Error response model for consistent API error responses
/// </summary>
public class ErrorResponse
{
  public HttpStatusCode StatusCode { get; set; }
  public string Message { get; set; } = string.Empty;
  public DateTime Timestamp { get; set; }
  public string? Details { get; set; }
  public string? StackTrace { get; set; }
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

