using trAInr.Domain.Entities;

namespace trAInr.Domain.Aggregates;

/// <summary>
///     ProgramTemplate aggregate root.
///     Represents a pre-made program template that can be cloned for athletes.
/// </summary>
public class ProgramTemplate
{
    private readonly List<ProgramTemplateWeek> _weeks = new();

    // Private constructor for EF Core
    private ProgramTemplate()
    {
        _weeks = new List<ProgramTemplateWeek>();
    }

    public ProgramTemplate(
        Guid id,
        string name,
        string description,
        int durationWeeks,
        ExperienceLevel experienceLevel,
        Guid createdBy,
        bool isUserGenerated)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Program template name cannot be empty", nameof(name));
        if (durationWeeks < 1 || durationWeeks > 52)
            throw new ArgumentException("Duration must be between 1 and 52 weeks", nameof(durationWeeks));

        Id = id;
        Name = name;
        Description = description;
        DurationWeeks = durationWeeks;
        ExperienceLevel = experienceLevel;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        IsUserGenerated = isUserGenerated;
    }

    public Guid Id { get; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int DurationWeeks { get; private set; }
    public ExperienceLevel ExperienceLevel { get; private set; }
    public bool IsActive { get; private set; }
    public Guid CreatedBy { get; private set; }
    public bool IsUserGenerated { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    public IReadOnlyCollection<ProgramTemplateWeek> Weeks => _weeks.AsReadOnly();

    /// <summary>
    ///     Adds a new week to the program template.
    /// </summary>
    public ProgramTemplateWeek AddWeek(int weekNumber, string? notes = null)
    {
        if (weekNumber < 1 || weekNumber > DurationWeeks)
            throw new ArgumentException($"Week number must be between 1 and {DurationWeeks}", nameof(weekNumber));

        if (_weeks.Any(w => w.WeekNumber == weekNumber))
            throw new InvalidOperationException($"Week {weekNumber} already exists in this program template");

        var week = new ProgramTemplateWeek
        {
            Id = Guid.NewGuid(),
            ProgramTemplateId = Id,
            WeekNumber = weekNumber,
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        };

        _weeks.Add(week);
        UpdatedAt = DateTime.UtcNow;

        return week;
    }

    /// <summary>
    ///     Adds a workout day to a specific week.
    /// </summary>
    public ProgramTemplateWorkoutDay? AddWorkoutDay(Guid weekId, ProgramTemplateWorkoutDay workoutDay)
    {
        var week = _weeks.FirstOrDefault(w => w.Id == weekId);
        if (week is null) return null;

        workoutDay.Id = Guid.NewGuid();
        workoutDay.ProgramTemplateWeekId = weekId;
        workoutDay.CreatedAt = DateTime.UtcNow;

        week.WorkoutDays.Add(workoutDay);
        UpdatedAt = DateTime.UtcNow;
        return workoutDay;
    }

    /// <summary>
    ///     Gets a week by its ID.
    /// </summary>
    public ProgramTemplateWeek? GetWeekById(Guid weekId)
    {
        return _weeks.FirstOrDefault(w => w.Id == weekId);
    }

    /// <summary>
    ///     Gets a week by its week number.
    /// </summary>
    public ProgramTemplateWeek? GetWeekByNumber(int weekNumber)
    {
        return _weeks.FirstOrDefault(w => w.WeekNumber == weekNumber);
    }

    /// <summary>
    ///     Deactivates the program template.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    ///     Activates the program template.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}