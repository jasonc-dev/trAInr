using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using trAInr.API.Middleware;
using trAInr.Application.Interfaces;
using trAInr.Application.Interfaces.Repositories;
using trAInr.Application.Interfaces.Services;
using trAInr.Application.Interfaces.Services.AI;
using trAInr.Application.Services;
using trAInr.Application.Services.AiProgramGenerator;
using trAInr.Application.Services.AssignedProgramme;
using trAInr.Application.Services.Exercise;
using trAInr.Application.Services.WorkoutSession;
using trAInr.Infrastructure.Api;
using trAInr.Infrastructure.Data;
using trAInr.Infrastructure.Repositories;
using trAInr.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configure JSON to properly serialize DateOnly types
        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
        // Serialize DateTime as ISO 8601 with UTC timezone
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// Configure API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddMvc().AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Configure OpenAPI (built-in .NET 10 support)
builder.Services.AddOpenApi();

// Configure database connection string
string connectionString;

// In production, require DATABASE_URL environment variable
if (builder.Environment.IsProduction())
{
    connectionString = builder.Configuration["DATABASE_URL"] ?? string.Empty;
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException(
            "Database connection string is not configured. " +
            "Please set the DATABASE_URL environment variable with your PostgreSQL connection string.");
    }
}
else
{
    // In development, use connection string from appsettings
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException(
            "Database connection string is not configured in appsettings.json.");
    }
}

builder.Services.AddDbContext<TrainrDbContext>(options =>
{
    options.UseNpgsql(connectionString, _ =>
    {
        // Enable DateOnly/TimeOnly mapping (native in .NET 6+)
    });
});

// Add memory cache for caching programme data
builder.Services.AddMemoryCache();

// Register cache provider
builder.Services.AddScoped<ICacheProvider, InMemoryCacheProvider>();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register DDD Aggregate Repositories
builder.Services.AddScoped<IAthleteRepository, AthleteRepository>();
builder.Services.AddScoped<IExerciseDefinitionRepository, ExerciseDefinitionRepository>();
builder.Services.AddScoped<IAssignedProgramRepository, AssignedProgramRepository>();
builder.Services.AddScoped<IProgramTemplateRepository, ProgramTemplateRepository>();
builder.Services.AddScoped<IWorkoutSessionRepository, WorkoutSessionRepository>();
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IIdempotencyRepository, IdempotencyRepository>();

// Register Application services
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAthleteService, AthleteService>();

// Register AssignedProgrammeService with caching decorator pattern
builder.Services.AddScoped<AssignedProgrammeService>();
builder.Services.AddScoped<IAssignedProgrammeService>(sp =>
{
    var innerService = sp.GetRequiredService<AssignedProgrammeService>();
    var cacheProvider = sp.GetRequiredService<ICacheProvider>();
    var logger = sp.GetRequiredService<ILogger<CachedAssignedProgrammeService>>();
    return new CachedAssignedProgrammeService(innerService, cacheProvider, logger);
});

// Register ExerciseDefinitionService with caching decorator pattern
builder.Services.AddScoped<ExerciseDefinitionService>();
builder.Services.AddScoped<IExerciseDefinitionService>(sp =>
{
    var innerService = sp.GetRequiredService<ExerciseDefinitionService>();
    var cacheProvider = sp.GetRequiredService<ICacheProvider>();
    var logger = sp.GetRequiredService<ILogger<CachedExerciseDefinitionService>>();
    return new CachedExerciseDefinitionService(innerService, cacheProvider, logger);
});

// Register WorkoutSessionService with caching decorator pattern
builder.Services.AddScoped<WorkoutSessionService>();
builder.Services.AddScoped<IWorkoutSessionService>(sp =>
{
    var innerService = sp.GetRequiredService<WorkoutSessionService>();
    var cacheProvider = sp.GetRequiredService<ICacheProvider>();
    var assignedProgramRepository = sp.GetRequiredService<IAssignedProgramRepository>();
    var logger = sp.GetRequiredService<ILogger<CachedWorkoutSessionService>>();
    return new CachedWorkoutSessionService(innerService, cacheProvider, assignedProgramRepository, logger);
});

builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAiProgramGeneratorService, AiProgramGeneratorService>();
builder.Services.AddScoped<IExerciseRetrievalService, ExerciseRetrievalService>();
builder.Services.AddScoped<ISyncService, SyncService>();

builder.Services.AddHostedService<AiProgramGenerationService>();

// Register OpenAI client
builder.Services.AddHttpClient<IOpenAiClient, OpenAiClient>(options =>
{
    options.Timeout = TimeSpan.FromMinutes(5);
});

// Register embedding service
builder.Services.AddHttpClient<IEmbeddingService, OpenAiEmbeddingService>(options =>
{
    options.Timeout = TimeSpan.FromMinutes(5);
});

// Configure CORS for frontend
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? ["http://localhost:3000"];

// Check for ALLOWED_ORIGINS environment variable (for Render.com and local mobile testing)
var allowedOriginsEnv = builder.Configuration["ALLOWED_ORIGINS"];
if (!string.IsNullOrEmpty(allowedOriginsEnv))
{
    allowedOrigins = allowedOriginsEnv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Configure Rate Limiting for mobile clients
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Global rate limit policy for authenticated users (100 requests per minute)
    options.AddPolicy("authenticated", context =>
    {
        var userId = context.Items["UserId"] as Guid?;
        var partitionKey = userId?.ToString() ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

        return RateLimitPartition.GetTokenBucketLimiter(partitionKey, _ => new TokenBucketRateLimiterOptions
        {
            TokenLimit = 100,
            ReplenishmentPeriod = TimeSpan.FromMinutes(1),
            TokensPerPeriod = 100,
            AutoReplenishment = true,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 10
        });
    });

    // Anonymous/unauthenticated rate limit (20 requests per minute per IP)
    options.AddPolicy("anonymous", context =>
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetTokenBucketLimiter(ipAddress, _ => new TokenBucketRateLimiterOptions
        {
            TokenLimit = 20,
            ReplenishmentPeriod = TimeSpan.FromMinutes(1),
            TokensPerPeriod = 20,
            AutoReplenishment = true,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 5
        });
    });

    // Default global limiter as fallback
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var userId = context.Items["UserId"] as Guid?;

        if (userId.HasValue)
        {
            return RateLimitPartition.GetTokenBucketLimiter(userId.Value.ToString(), _ => new TokenBucketRateLimiterOptions
            {
                TokenLimit = 100,
                ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                TokensPerPeriod = 100,
                AutoReplenishment = true
            });
        }

        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetTokenBucketLimiter(ipAddress, _ => new TokenBucketRateLimiterOptions
        {
            TokenLimit = 20,
            ReplenishmentPeriod = TimeSpan.FromMinutes(1),
            TokensPerPeriod = 20,
            AutoReplenishment = true
        });
    });

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.Headers.RetryAfter = "60";

        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            code = "RATE_LIMIT_EXCEEDED",
            message = "Too many requests. Please try again later.",
            retryAfterSeconds = 60,
            retryable = true
        }, cancellationToken);
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline
// Map OpenAPI document endpoint
app.MapOpenApi();

// Map Scalar API documentation UI
app.MapScalarApiReference(options =>
{
    options.WithTitle("trAInr API");
    options.WithTheme(ScalarTheme.BluePlanet);
});

app.UseCors("AllowFrontend");

// Global exception handler - must be registered early to catch all exceptions
// Placed after CORS to ensure CORS headers are available for error responses
app.UseGlobalExceptionHandler();

// Add JWT authentication middleware
app.UseJwtAuthentication();

// Add rate limiting after authentication so user ID is available
app.UseRateLimiter();

app.UseAuthorization();
app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TrainrDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        await dbContext.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred applying database migrations.");
        throw;
    }
}



app.Run();


/// <summary>
///     JSON converter for DateOnly type to/from ISO 8601 date string (yyyy-MM-dd)
/// </summary>
public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    private const string DateFormat = "yyyy-MM-dd";

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateString = reader.GetString();
        if (string.IsNullOrEmpty(dateString)) return default;
        return DateOnly.ParseExact(dateString, DateFormat, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
    }
}