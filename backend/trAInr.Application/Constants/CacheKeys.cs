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

    #region Exercise Definition Cache Keys

    /// <summary>
    /// Cache key for all exercise definitions.
    /// Pattern: "exercise:all"
    /// </summary>
    public const string AllExercises = "exercise:all";

    /// <summary>
    /// Cache key for a specific exercise definition by ID.
    /// Pattern: "exercise:id:{exerciseId}"
    /// </summary>
    public static string ExerciseById(int exerciseId) => $"exercise:id:{exerciseId}";

    /// <summary>
    /// Cache key for exercises by type.
    /// Pattern: "exercise:type:{exerciseType}"
    /// </summary>
    public static string ExercisesByType(string exerciseType) => $"exercise:type:{exerciseType}";

    /// <summary>
    /// Cache key for exercises by muscle group.
    /// Pattern: "exercise:musclegroup:{muscleGroup}"
    /// </summary>
    public static string ExercisesByMuscleGroup(string muscleGroup) => $"exercise:musclegroup:{muscleGroup}";

    /// <summary>
    /// Cache key for exercise search results.
    /// Pattern: "exercise:search:{query}:{type}:{muscleGroup}"
    /// </summary>
    public static string ExerciseSearch(string? query, string? type, string? muscleGroup)
        => $"exercise:search:{query ?? "all"}:{type ?? "all"}:{muscleGroup ?? "all"}";

    #endregion
}
