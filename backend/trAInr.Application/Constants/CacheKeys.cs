namespace trAInr.Application.Constants;

/// <summary>
/// Defines standardized cache key patterns for programme-related data.
/// Keys are structured with prefixes to enable efficient bulk invalidation.
/// </summary>
public static class CacheKeys
{
    #region Programme Cache Keys
    /// <summary>
    /// Cache key for all assigned programmes for a specific athlete.
    /// Pattern: "programme:athlete:{athleteId}"
    /// </summary>
    public static string ProgrammesByAthlete(Guid athleteId) => $"programme:athlete:{athleteId}";

    /// <summary>
    /// Cache key for the active programme for a specific athlete.
    /// Pattern: "programme:active:{athleteId}"
    /// </summary>
    public static string ActiveProgrammeByAthlete(Guid athleteId) => $"programme:active:{athleteId}";

    /// <summary>
    /// Cache key for a specific programme by ID.
    /// Pattern: "programme:id:{programmeId}"
    /// </summary>
    public static string ProgrammeById(Guid programmeId) => $"programme:id:{programmeId}";

    /// <summary>
    /// Cache key for all pre-made programme templates.
    /// Pattern: "programme:templates:premade"
    /// </summary>
    public const string PreMadeProgrammeTemplates = "programme:templates:premade";

    /// <summary>
    /// Cache key for programme templates created by a specific athlete.
    /// Pattern: "programme:templates:created:{athleteId}"
    /// </summary>
    public static string CreatedProgrammeTemplates(Guid athleteId) => $"programme:templates:created:{athleteId}";

    #endregion

    #region Workout Session Cache Keys

    /// <summary>
    /// Cache key for a specific workout day by ID.
    /// Pattern: "workout:day:id:{workoutDayId}"
    /// </summary>
    public static string WorkoutDayById(Guid workoutDayId) => $"workout:day:id:{workoutDayId}";

    /// <summary>
    /// Cache key for a specific workout exercise by ID.
    /// Pattern: "workout:exercise:id:{workoutExerciseId}"
    /// </summary>
    public static string WorkoutExerciseById(Guid workoutExerciseId) => $"workout:exercise:id:{workoutExerciseId}";

    /// <summary>
    /// Cache key for a specific exercise set by ID.
    /// Pattern: "workout:set:id:{exerciseSetId}"
    /// </summary>
    public static string ExerciseSetById(Guid exerciseSetId) => $"workout:set:id:{exerciseSetId}";

    #endregion
}
