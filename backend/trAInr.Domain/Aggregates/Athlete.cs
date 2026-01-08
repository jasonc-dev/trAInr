using trAInr.Domain.Events;
using trAInr.Domain.ValueObjects;

namespace trAInr.Domain.Aggregates;

/// <summary>
///     Athlete aggregate root.
///     Represents a user/athlete with their training profile, constraints, and preferences.
/// </summary>
public class Athlete
{
    private readonly List<string> _constraints = new();
    private readonly List<DomainEvent> _domainEvents = new();
    private readonly List<EquipmentRequirement> _equipmentPreferences = new();

    // Private constructor for EF Core
    private Athlete()
    {
    }

    public Athlete(
        Guid id,
        string username,
        string passwordHash,
        string email,
        string firstName,
        string lastName,
        DateOnly dateOfBirth,
        TrainingLevel level,
        FitnessGoal primaryGoal,
        int workoutDaysPerWeek)
    {
        Id = id;
        Username = username;
        PasswordHash = passwordHash;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Level = level;
        PrimaryGoal = primaryGoal;
        WorkoutDaysPerWeek = workoutDaysPerWeek;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; }
    public string Username { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public DateOnly DateOfBirth { get; private set; }

    // Training profile
    public TrainingLevel Level { get; private set; }
    public FitnessGoal PrimaryGoal { get; private set; }
    public int WorkoutDaysPerWeek { get; private set; }

    // Readiness and constraints
    public decimal? ReadinessScore { get; private set; }
    public string? ReadinessNotes { get; private set; }
    public IReadOnlyCollection<string> Constraints => _constraints.AsReadOnly();
    public IReadOnlyCollection<EquipmentRequirement> EquipmentPreferences => _equipmentPreferences.AsReadOnly();

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    // Domain events
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    ///     Adjusts the athlete's training level and raises a domain event.
    /// </summary>
    public void AdjustTrainingLevel(TrainingLevel newLevel, string reason)
    {
        if (Level == newLevel) return;

        var previousLevel = Level.ToString();
        Level = newLevel;
        UpdatedAt = DateTime.UtcNow;

        _domainEvents.Add(new TrainingLevelAdjusted(
            Id,
            previousLevel,
            newLevel.ToString(),
            reason
        ));
    }

    /// <summary>
    ///     Updates the athlete's readiness score and raises a domain event.
    /// </summary>
    public void UpdateReadiness(decimal score, string? notes = null)
    {
        if (score is < 0 or > 100)
            throw new ArgumentException("Readiness score must be between 0 and 100", nameof(score));

        ReadinessScore = score;
        ReadinessNotes = notes;
        UpdatedAt = DateTime.UtcNow;

        _domainEvents.Add(new ReadinessUpdated(Id, score, notes));
    }

    /// <summary>
    ///     Adds a constraint to the athlete's profile.
    /// </summary>
    public void AddConstraint(string constraint)
    {
        if (string.IsNullOrWhiteSpace(constraint))
            throw new ArgumentException("Constraint cannot be empty", nameof(constraint));

        if (!_constraints.Contains(constraint))
        {
            _constraints.Add(constraint);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    ///     Removes a constraint from the athlete's profile.
    /// </summary>
    public void RemoveConstraint(string constraint)
    {
        _constraints.Remove(constraint);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    ///     Adds an equipment preference.
    /// </summary>
    public void AddEquipmentPreference(EquipmentRequirement equipment)
    {
        if (_equipmentPreferences.All(e => e.Name != equipment.Name))
        {
            _equipmentPreferences.Add(equipment);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    ///     Updates the athlete's profile information.
    /// </summary>
    public void UpdateProfile(
        string firstName,
        string lastName,
        FitnessGoal primaryGoal,
        int workoutDaysPerWeek)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));
        if (workoutDaysPerWeek < 1 || workoutDaysPerWeek > 7)
            throw new ArgumentException("Workout days per week must be between 1 and 7", nameof(workoutDaysPerWeek));

        FirstName = firstName;
        LastName = lastName;
        PrimaryGoal = primaryGoal;
        WorkoutDaysPerWeek = workoutDaysPerWeek;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    ///     Clears all domain events (typically called after persistence).
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

/// <summary>
///     Training level enum for athlete classification.
/// </summary>
public enum TrainingLevel
{
    Beginner = 1,
    Intermediate = 2,
    Advanced = 3,
    Elite = 4
}

/// <summary>
///     Primary fitness goal enum.
/// </summary>
public enum FitnessGoal
{
    BuildMuscle = 1,
    LoseWeight = 2,
    ImproveEndurance = 3,
    IncreaseStrength = 4,
    GeneralFitness = 5
}