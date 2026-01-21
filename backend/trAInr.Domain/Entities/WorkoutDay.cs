namespace trAInr.Domain.Entities;

/// <summary>
///     Represents a workout day within a week
/// </summary>
public class WorkoutDay
{
    public Guid Id { get; set; }
    public Guid ProgrammeWeekId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>
    ///     Scheduled date for this workout (date only, no time component)
    /// </summary>
    public DateOnly? ScheduledDate { get; set; }

    /// <summary>
    ///     When the workout was completed (UTC timestamp)
    /// </summary>
    public DateTime? CompletedDate { get; set; }

    public bool IsCompleted { get; set; }
    public bool IsRestDay { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ProgrammeWeek ProgrammeWeek { get; set; } = null!;
    public ICollection<WorkoutExercise> Exercises { get; set; } = new List<WorkoutExercise>();

    public void Complete(DateTime? completedAt = null)
    {
        IsCompleted = true;
        CompletedDate = completedAt ?? DateTime.UtcNow;
    }

    public WorkoutExercise AddExercise(
        Guid exerciseId,
        int orderIndex,
        int targetSets,
        int targetReps,
        decimal? targetWeight,
        int? targetDurationSeconds,
        decimal? targetDistance,
        int? restSeconds,
        int? targetRpe,
        string? notes)
    {
        var nextOrderIndex = orderIndex < 0 ? Exercises.Count : orderIndex;

        var exercise = new WorkoutExercise
        {
            Id = Guid.NewGuid(),
            WorkoutDayId = Id,
            ExerciseDefinitionId = exerciseId,
            OrderIndex = nextOrderIndex,
            Notes = notes,
            TargetSets = targetSets,
            TargetReps = targetReps,
            TargetWeight = targetWeight,
            TargetDurationSeconds = targetDurationSeconds,
            TargetDistance = targetDistance,
            RestSeconds = restSeconds,
            TargetRpe = targetRpe,
            CreatedAt = DateTime.UtcNow,
            Sets = CreateSets(targetSets, targetReps, targetWeight, targetDurationSeconds, notes)
        };

        Exercises.Add(exercise);
        return exercise;
    }

    public bool RemoveExercise(Guid exerciseId)
    {
        var exercise = Exercises.FirstOrDefault(e => e.Id == exerciseId);
        if (exercise is null) return false;

        Exercises.Remove(exercise);

        var orderedExercises = Exercises.OrderBy(e => e.OrderIndex).ToList();
        for (var i = 0; i < orderedExercises.Count; i++)
        {
            orderedExercises[i].OrderIndex = i;
        }

        return true;
    }

    public bool ReorderExercises(IEnumerable<Guid> exerciseIds)
    {
        var idList = exerciseIds.ToList();
        if (idList.Count != Exercises.Count) return false;

        var lookup = Exercises.ToDictionary(e => e.Id);
        if (idList.Any(id => !lookup.ContainsKey(id))) return false;

        for (var i = 0; i < idList.Count; i++)
        {
            lookup[idList[i]].OrderIndex = i;
        }

        return true;
    }

        private static List<ExerciseSet> CreateSets(int targetSets, int targetReps, decimal? targetWeight, int? targetDurationSeconds, string? notes)
    {
        var sets = new List<ExerciseSet>();
        for (var i = 0; i < targetSets; i++)
        {
            var set = new ExerciseSet()
            {
                Id = Guid.NewGuid(),
                WorkoutExerciseId = new Guid(),
                SetNumber = i + 1,
                Reps = targetReps,
                Weight = targetWeight,
                DurationSeconds = targetDurationSeconds,
                IsCompleted = false,
                SetType = SetType.Normal,
                DropPercentage = null,
                Notes = notes,
                CreatedAt = DateTime.UtcNow,
                CompletedAt = null,

            };
            sets.Add(set);
        }

        return sets;
    }
}