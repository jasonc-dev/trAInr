using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using trAInr.API.Data;
using trAInr.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure OpenAPI (built-in .NET 10 support)
builder.Services.AddOpenApi();

// Configure database based on environment
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<TrainrDbContext>(options => { options.UseNpgsql(connectionString); });

// Register services
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

app.UseAuthorization();
app.MapControllers();

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TrainrDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Ensuring database is created...");
        dbContext.Database.EnsureCreated();
        logger.LogInformation("Database ready.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while creating the database.");
    }
}

app.Run();