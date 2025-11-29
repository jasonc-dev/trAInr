using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using trAInr.API.Data;
using trAInr.API.Middleware;
using trAInr.API.Services;

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

builder.Services.AddDbContext<TrainrDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        // Enable DateOnly/TimeOnly mapping (native in .NET 6+)
    });
});

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProgrammeService, ProgrammeService>();
builder.Services.AddScoped<IExerciseService, ExerciseService>();
builder.Services.AddScoped<IWorkoutService, WorkoutService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// Configure CORS for frontend
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
                     ?? new[] { "http://localhost:3000" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
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

// Only use HTTPS redirection in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Add JWT authentication middleware
app.UseJwtAuthentication();

app.UseAuthorization();
app.MapControllers();

// Apply database migrations and fix date column types
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TrainrDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // First, ensure the database exists
        dbContext.Database.EnsureCreated();
        
        // Check if date columns need to be converted from timestamp to date
        // This handles databases created before the DateOnly migration
        var needsDateConversion = false;
        using (var connection = dbContext.Database.GetDbConnection())
        {
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT COUNT(*) FROM information_schema.columns 
                WHERE table_name = 'Users' 
                AND column_name = 'DateOfBirth' 
                AND data_type = 'timestamp with time zone'";
            var result = command.ExecuteScalar();
            needsDateConversion = Convert.ToInt32(result) > 0;
        }
        
        if (needsDateConversion)
        {
            logger.LogInformation("Converting date columns from timestamp to date type...");
            
            // Convert timestamp columns to date columns
            dbContext.Database.ExecuteSqlRaw(@"
                ALTER TABLE ""Users"" ALTER COLUMN ""DateOfBirth"" TYPE date USING ""DateOfBirth""::date;
                ALTER TABLE ""Programmes"" ALTER COLUMN ""StartDate"" TYPE date USING ""StartDate""::date;
                ALTER TABLE ""Programmes"" ALTER COLUMN ""EndDate"" TYPE date USING ""EndDate""::date;
                ALTER TABLE ""WorkoutDays"" ALTER COLUMN ""ScheduledDate"" TYPE date USING ""ScheduledDate""::date;
            ");
            
            logger.LogInformation("Date columns converted successfully.");
        }
        else
        {
            logger.LogInformation("Database schema is up to date.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while setting up the database.");
        throw;
    }
}

app.Run();

/// <summary>
/// JSON converter for DateOnly type to/from ISO 8601 date string (yyyy-MM-dd)
/// </summary>
public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    private const string DateFormat = "yyyy-MM-dd";

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateString = reader.GetString();
        if (string.IsNullOrEmpty(dateString))
        {
            return default;
        }
        return DateOnly.ParseExact(dateString, DateFormat, System.Globalization.CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateFormat, System.Globalization.CultureInfo.InvariantCulture));
    }
}