namespace trAInr.API.Models.Domain;

/// <summary>
/// Represents a user of the application
/// </summary>
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public FitnessLevel FitnessLevel { get; set; }
    public FitnessGoal PrimaryGoal { get; set; }
    public int WorkoutDaysPerWeek { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<Programme> Programmes { get; set; } = new List<Programme>();
}

/// <summary>
/// User's current fitness level for programme recommendations
/// </summary>
public enum FitnessLevel
{
    Beginner = 1,
    Intermediate = 2,
    Advanced = 3,
    Elite = 4
}

/// <summary>
/// User's primary fitness goal
/// </summary>
public enum FitnessGoal
{
    BuildMuscle = 1,
    LoseWeight = 2,
    ImproveEndurance = 3,
    IncreaseStrength = 4,
    GeneralFitness = 5
}

