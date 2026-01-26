using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using trAInr.API.Middleware;
using trAInr.Application.Interfaces;
using trAInr.Application.Interfaces.Repositories;
using trAInr.Application.Interfaces.Services;
using trAInr.Application.Interfaces.Services.AI;
using trAInr.Application.Services;
using trAInr.Application.Services.AI;
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
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        // Enable DateOnly/TimeOnly mapping (native in .NET 6+)
    });
});

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register DDD Aggregate Repositories
builder.Services.AddScoped<IAthleteRepository, AthleteRepository>();
builder.Services.AddScoped<IExerciseDefinitionRepository, ExerciseDefinitionRepository>();
builder.Services.AddScoped<IAssignedProgramRepository, AssignedProgramRepository>();
builder.Services.AddScoped<IProgramTemplateRepository, ProgramTemplateRepository>();
builder.Services.AddScoped<IWorkoutSessionRepository, WorkoutSessionRepository>();

// Register Application services
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAthleteService, AthleteService>();
builder.Services.AddScoped<IAssignedProgrammeService, AssignedProgrammeService>();
builder.Services.AddScoped<IExerciseDefinitionService, ExerciseDefinitionService>();
builder.Services.AddScoped<IWorkoutSessionService, WorkoutSessionService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAiProgramGeneratorService, AiProgramGeneratorService>();

// Register OpenAI client
builder.Services.AddHttpClient<IOpenAiClient, OpenAiClient>(options =>
{
    options.Timeout = TimeSpan.FromMinutes(5);
});

// Configure CORS for frontend
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:3000" };

// Check for ALLOWED_ORIGINS environment variable (for Render.com)
var allowedOriginsEnv = builder.Configuration["ALLOWED_ORIGINS"];
if (!string.IsNullOrEmpty(allowedOriginsEnv))
{
    allowedOrigins = allowedOriginsEnv.Split(',');
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

// Only use HTTPS redirection in production
if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();

// Add JWT authentication middleware
app.UseJwtAuthentication();

app.UseAuthorization();
app.MapControllers();


// Apply database migrations and seed data on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TrainrDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Applying database migrations...");
        dbContext.Database.Migrate();
        logger.LogInformation("Database migrations completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during database initialization.");
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