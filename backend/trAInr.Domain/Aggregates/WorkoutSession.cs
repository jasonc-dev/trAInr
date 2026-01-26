using trAInr.Domain.Events;

namespace trAInr.Domain.Aggregates;

/// <summary>
///     WorkoutSession aggregate root.
///     Represents a single workout session with exercise instances and completed sets.
/// </summary>
public class WorkoutSession
{
    private readonly List<DomainEvent> _domainEvents = new();
    private readonly List<ExerciseInstance> _exerciseInstances = new();

    // Private constructor for EF Core
    private WorkoutSession()
    {
    }

    public WorkoutSession(
        Guid id,
        Guid athleteId,
        DayOfWeek dayOfWeek,
        string name,
        Guid? programmeWeekId = null,
        string? description = null,
        DateOnly? scheduledDate = null,
        bool isRestDay = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Workout name cannot be empty", nameof(name));

        Id = id;
        AthleteId = athleteId;
        ProgrammeWeekId = programmeWeekId;
        DayOfWeek = dayOfWeek;
        Name = name;
        Description = description;
        ScheduledDate = scheduledDate;
        IsRestDay = isRestDay;
        IsCompleted = false;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; }
    public Guid AthleteId { get; }
    public Guid? ProgrammeWeekId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateOnly? ScheduledDate { get; }
    public DateTime? CompletedDate { get; private set; }
    public bool IsCompleted { get; private set; }
    public bool IsRestDay { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public IReadOnlyCollection<ExerciseInstance> ExerciseInstances => _exerciseInstances.AsReadOnly();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    ///     Adds a completed set to an exercise instance and raises a domain event.
    /// </summary>
    public void AddCompletedSet(
        Guid exerciseInstanceId,
        int setNumber,
        int? reps,
        decimal? weight,
        int? rpe,
        int? durationSeconds = null,
        decimal? distance = null,
        string? notes = null)
    {
        var exerciseInstance = _exerciseInstances.FirstOrDefault(e => e.Id == exerciseInstanceId);
        if (exerciseInstance == null)
            throw new InvalidOperationException($"Exercise instance {exerciseInstanceId} not found in this session");

        var completedSet = exerciseInstance.AddCompletedSet(
            setNumber,
            reps,
            weight,
            rpe,
            durationSeconds,
            distance,
            notes);

        _domainEvents.Add(new SetLogged(
            completedSet.Id,
            Id,
            exerciseInstanceId,
            reps ?? 0,
            weight,
            rpe
        ));
    }

    /// <summary>
    ///     Adds an exercise instance to the session.
    /// </summary>
    public ExerciseInstance AddExerciseInstance(
        int exerciseDefinitionId,
        string exerciseName,
        int orderIndex,
        int targetSets,
        int targetReps,
        decimal? targetWeight = null,
        int? targetDurationSeconds = null,
        decimal? targetDistance = null,
        string? notes = null)
    {
        var exerciseInstance = new ExerciseInstance(
            Guid.NewGuid(),
            Id,
            exerciseDefinitionId,
            exerciseName,
            orderIndex,
            targetSets,
            targetReps,
            targetWeight,
            targetDurationSeconds,
            targetDistance,
            notes);

        _exerciseInstances.Add(exerciseInstance);
        return exerciseInstance;
    }

    /// <summary>
    ///     Finalizes the workout session and raises a domain event.
    /// </summary>
    public void FinalizeSession(DateTime? completedAt = null)
    {
        if (IsCompleted)
            throw new InvalidOperationException("Session is already completed");

        IsCompleted = true;
        CompletedDate = completedAt ?? DateTime.UtcNow;

        var totalSets = _exerciseInstances.Sum(e => e.CompletedSets.Count);
        var totalVolume = _exerciseInstances
            .SelectMany(e => e.CompletedSets)
            .Where(s => s.Weight.HasValue && s.Reps.HasValue)
            .Sum(s => s.Weight!.Value * s.Reps!.Value);

        _domainEvents.Add(new SessionCompleted(
            Id,
            AthleteId,
            ScheduledDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
            totalSets,
            totalVolume
        ));
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
///     Exercise instance within a workout session.
/// </summary>
public class ExerciseInstance
{
    private readonly List<CompletedSet> _completedSets = new();

    // Private constructor for EF Core
    private ExerciseInstance()
    {
    }

    public ExerciseInstance(
        Guid id,
        Guid workoutSessionId,
        int exerciseDefinitionId,
        string exerciseName,
        int orderIndex,
        int targetSets,
        int targetReps,
        decimal? targetWeight = null,
        int? targetDurationSeconds = null,
        decimal? targetDistance = null,
        string? notes = null)
    {
        Id = id;
        WorkoutSessionId = workoutSessionId;
        ExerciseDefinitionId = exerciseDefinitionId;
        ExerciseName = exerciseName;
        OrderIndex = orderIndex;
        TargetSets = targetSets;
        TargetReps = targetReps;
        TargetWeight = targetWeight;
        TargetDurationSeconds = targetDurationSeconds;
        TargetDistance = targetDistance;
        Notes = notes;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; }
    public Guid WorkoutSessionId { get; private set; }
    public int ExerciseDefinitionId { get; private set; }
    public string ExerciseName { get; private set; } = string.Empty;
    public int OrderIndex { get; private set; }
    public int TargetSets { get; private set; }
    public int TargetReps { get; private set; }
    public decimal? TargetWeight { get; private set; }
    public int? TargetDurationSeconds { get; private set; }
    public decimal? TargetDistance { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public IReadOnlyCollection<CompletedSet> CompletedSets => _completedSets.AsReadOnly();

    public CompletedSet AddCompletedSet(
        int setNumber,
        int? reps,
        decimal? weight,
        int? rpe,
        int? durationSeconds = null,
        decimal? distance = null,
        string? notes = null)
    {
        var completedSet = new CompletedSet(
            Guid.NewGuid(),
            Id,
            setNumber,
            reps,
            weight,
            rpe,
            durationSeconds,
            distance,
            notes);

        _completedSets.Add(completedSet);
        return completedSet;
    }
}

/// <summary>
///     Completed set within an exercise instance.
/// </summary>
public class CompletedSet
{
    // Private constructor for EF Core
    private CompletedSet()
    {
    }

    public CompletedSet(
        Guid id,
        Guid exerciseInstanceId,
        int setNumber,
        int? reps,
        decimal? weight,
        int? rpe,
        int? durationSeconds = null,
        decimal? distance = null,
        string? notes = null,
        bool isWarmup = false)
    {
        Id = id;
        ExerciseInstanceId = exerciseInstanceId;
        SetNumber = setNumber;
        Reps = reps;
        Weight = weight;
        RPE = rpe;
        DurationSeconds = durationSeconds;
        Distance = distance;
        Notes = notes;
        IsWarmup = isWarmup;
        IsCompleted = true;
        CompletedAt = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; }
    public Guid ExerciseInstanceId { get; private set; }
    public int SetNumber { get; private set; }
    public int? Reps { get; }
    public decimal? Weight { get; }
    public int? RPE { get; private set; }
    public int? DurationSeconds { get; private set; }
    public decimal? Distance { get; private set; }
    public string? Notes { get; private set; }
    public bool IsCompleted { get; private set; }
    public bool IsWarmup { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}