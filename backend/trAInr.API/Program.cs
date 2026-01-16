using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using trAInr.API.Middleware;
using trAInr.Application.Interfaces;
using trAInr.Application.Interfaces.Repositories;
using trAInr.Application.Interfaces.Services;
using trAInr.Application.Services;
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

// Configure database based on environment
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Allow DATABASE_URL environment variable to override connection string (for Render.com)
var databaseUrl = builder.Configuration["DATABASE_URL"];
if (!string.IsNullOrEmpty(databaseUrl))
{
    connectionString = databaseUrl;
}

builder.Services.AddDbContext<TrainrDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        // Enable DateOnly/TimeOnly mapping (native in .NET 6+)
    });
});

// Register Infrastructure services
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register DDD Aggregate Repositories
builder.Services.AddScoped<IAthleteRepository, AthleteRepository>();
builder.Services.AddScoped<IExerciseDefinitionRepository, ExerciseDefinitionRepository>();
builder.Services.AddScoped<IAssignedProgramRepository, AssignedProgramRepository>();
builder.Services.AddScoped<IWorkoutSessionRepository, WorkoutSessionRepository>();

// Register Application services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAthleteService, AthleteService>();
builder.Services.AddScoped<IAssignedProgrammeService, AssignedProgrammeService>();
builder.Services.AddScoped<IExerciseDefinitionService, ExerciseDefinitionService>();
builder.Services.AddScoped<IWorkoutSessionService, WorkoutSessionService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

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

        // Seed exercise definitions if table is empty
        await SeedExerciseDefinitionsAsync(dbContext, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during database initialization.");
        throw;
    }
}

/// <summary>
/// Seeds exercise definitions from SQL file if the table is empty.
/// </summary>
static async Task SeedExerciseDefinitionsAsync(TrainrDbContext dbContext, ILogger logger)
{
    try
    {
        // Check if ExerciseDefinitions table has any data
        var exerciseCount = await dbContext.ExerciseDefinitions.CountAsync();

        if (exerciseCount == 0)
        {
            logger.LogInformation("Seeding exercise definitions...");

            // Read the SQL file content
            var sqlFilePath = Path.Combine(AppContext.BaseDirectory, "Data", "insert_exercise_definitions.sql");

            if (File.Exists(sqlFilePath))
            {
                var sqlContent = await File.ReadAllTextAsync(sqlFilePath);
                await dbContext.Database.ExecuteSqlRawAsync(sqlContent);
                logger.LogInformation("Exercise definitions seeded successfully.");
            }
            else
            {
                logger.LogWarning("Exercise definitions SQL file not found at: {Path}", sqlFilePath);
            }
        }
        else
        {
            logger.LogInformation("Exercise definitions already exist ({Count} records), skipping seed.", exerciseCount);
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error occurred while seeding exercise definitions.");
        // Don't throw here - allow application to continue even if seeding fails
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