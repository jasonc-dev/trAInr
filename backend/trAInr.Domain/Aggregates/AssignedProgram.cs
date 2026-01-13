using trAInr.Domain.Entities;
using trAInr.Domain.Events;

namespace trAInr.Domain.Aggregates;

/// <summary>
///     AssignedProgram aggregate root.
///     Represents a program template assigned to an athlete.
/// </summary>
public class AssignedProgram
{
    private readonly List<DomainEvent> _domainEvents = new();
    private readonly List<ProgrammeWeek> _weeks = new();

    // Private constructor for EF Core
    private AssignedProgram()
    {
        _weeks = new List<ProgrammeWeek>();
    }

    public AssignedProgram(
        Guid id,
        Guid athleteId,
        Guid programTemplateId,
        string name,
        string description,
        int durationWeeks,
        DateOnly startDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Program name cannot be empty", nameof(name));
        if (durationWeeks < 1 || durationWeeks > 52)
            throw new ArgumentException("Duration must be between 1 and 52 weeks", nameof(durationWeeks));

        Id = id;
        AthleteId = athleteId;
        ProgramTemplateId = programTemplateId;
        Name = name;
        Description = description;
        DurationWeeks = durationWeeks;
        StartDate = startDate;
        EndDate = startDate.AddDays(durationWeeks * 7 - 1);
        IsActive = true;
        CurrentPhase = 1;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        _domainEvents.Add(new ProgramAssigned(
            Id,
            AthleteId,
            ProgramTemplateId,
            StartDate
        ));
    }

    public Guid Id { get; }
    public Guid AthleteId { get; }
    public Guid ProgramTemplateId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int DurationWeeks { get; private set; }
    public int CurrentPhase { get; private set; } = 1;
    public DateOnly StartDate { get; }
    public DateOnly? EndDate { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    public IReadOnlyCollection<ProgrammeWeek> Weeks => _weeks.AsReadOnly();

    /// <summary>
    ///     Advances to the next phase if progression rules are satisfied.
    /// </summary>
    public void AdvancePhase(int newPhase, bool validateProgression = true)
    {
        if (newPhase <= CurrentPhase)
            throw new InvalidOperationException("New phase must be greater than current phase");
        if (newPhase > DurationWeeks)
            throw new InvalidOperationException("Phase cannot exceed program duration");

        if (validateProgression && !ValidateProgression(newPhase))
            throw new InvalidOperationException("Progression rules not satisfied");

        var previousPhase = CurrentPhase;
        CurrentPhase = newPhase;
        UpdatedAt = DateTime.UtcNow;

        _domainEvents.Add(new ProgramPhaseCompleted(
            Id,
            AthleteId,
            previousPhase,
            DateOnly.FromDateTime(DateTime.UtcNow)
        ));
    }

    /// <summary>
    ///     Validates that progression rules are satisfied for advancing to a phase.
    /// </summary>
    public bool ValidateProgression(int targetPhase)
    {
        // Basic validation: ensure we're not skipping phases
        if (targetPhase != CurrentPhase + 1)
            return false;

        // Additional validation logic can be added here
        // For example, checking if previous phase workouts are completed
        return true;
    }

    /// <summary>
    ///     Resets the program to the beginning.
    /// </summary>
    public void Reset()
    {
        CurrentPhase = 1;
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    ///     Activates the program.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    ///     Deactivates the program.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    ///     Adds a new week to the program.
    /// </summary>
    public ProgrammeWeek AddWeek(int weekNumber, string? notes = null)
    {
        if (weekNumber < 1 || weekNumber > DurationWeeks)
            throw new ArgumentException($"Week number must be between 1 and {DurationWeeks}", nameof(weekNumber));

        if (_weeks.Any(w => w.WeekNumber == weekNumber))
            throw new InvalidOperationException($"Week {weekNumber} already exists in this program");

        var weekStartDate = StartDate.AddDays((weekNumber - 1) * 7);

        var week = new ProgrammeWeek
        {
            Id = Guid.NewGuid(),
            AssignedProgramId = Id,
            WeekStartDate = weekStartDate,
            WeekNumber = weekNumber,    
            Notes = notes,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _weeks.Add(week);
        UpdatedAt = DateTime.UtcNow;

        return week;
    }

    /// <summary>
    ///     Adds a workout day to a specific week.
    /// </summary>
    public WorkoutDay? AddWorkoutDay(Guid weekId, WorkoutDay workoutDay)
    {
        var week = _weeks.FirstOrDefault(w => w.Id == weekId);
        if (week is null) return null;

        var scheduledDate = week.WeekStartDate.AddDays((int)workoutDay.DayOfWeek - 1);
        
        workoutDay.Id = Guid.NewGuid();
        workoutDay.ProgrammeWeekId = weekId;
        workoutDay.CreatedAt = DateTime.UtcNow;
        workoutDay.ScheduledDate = scheduledDate;
        
        week.WorkoutDays.Add(workoutDay);
        UpdatedAt = DateTime.UtcNow;
        return workoutDay;
    }

    /// <summary>
    ///     Updates an existing week in the program.
    /// </summary>
    public ProgrammeWeek? UpdateWeek(Guid weekId, bool? isCompleted = null)
    {
        var week = _weeks.FirstOrDefault(w => w.Id == weekId);
        if (week is null)
            return null;

        if (isCompleted.HasValue)
        {
            week.IsCompleted = isCompleted.Value;
            if (isCompleted.Value && week.WeekNumber == CurrentPhase)
            {
                // Auto-advance phase if current week is completed
                if (CurrentPhase < DurationWeeks)
                {
                    AdvancePhase(CurrentPhase + 1, validateProgression: false);
                }
            }
        }

        UpdatedAt = DateTime.UtcNow;
        return week;
    }

    /// <summary>
    ///     Gets a week by its ID.
    /// </summary>
    public ProgrammeWeek? GetWeekById(Guid weekId)
    {
        return _weeks.FirstOrDefault(w => w.Id == weekId);
    }

    /// <summary>
    ///     Gets a week by its week number.
    /// </summary>
    public ProgrammeWeek? GetWeekByNumber(int weekNumber)
    {
        return _weeks.FirstOrDefault(w => w.WeekNumber == weekNumber);
    }

    /// <summary>
    ///     Clears all domain events (typically called after persistence).
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}