using trAInr.Application.DTOs;

namespace trAInr.Application.Interfaces.Services;

public interface IWorkoutSessionService
{
    Task<WorkoutDayResponse?> GetWorkoutDayAsync(Guid workoutDayId);

    // Workout Day operations - return ProgrammeWeekResponse
    Task<ProgrammeWeekResponse?> CreateWorkoutDayAsync(Guid weekId, CreateWorkoutDayRequest request);

    Task<ProgrammeWeekResponse?> UpdateWorkoutDayAsync(Guid workoutDayId, UpdateWorkoutDayRequest request);

    Task<ProgrammeWeekResponse?> DeleteWorkoutDayAsync(Guid workoutDayId);

    Task<ProgrammeWeekResponse?> CompleteWorkoutAsync(Guid workoutDayId, CompleteWorkoutRequest request);

    // Exercise operations - return WorkoutDayResponse
    Task<WorkoutDayResponse?> AddExerciseToWorkoutAsync(Guid workoutDayId, AddWorkoutExerciseRequest request);

    Task<WorkoutDayResponse?> UpdateWorkoutExerciseAsync(Guid workoutExerciseId,
        UpdateWorkoutExerciseRequest request);

    Task<WorkoutDayResponse?> RemoveExerciseFromWorkoutAsync(Guid workoutExerciseId);

    Task<WorkoutDayResponse?> ReorderExercisesAsync(Guid workoutDayId, List<Guid> workoutExerciseIds);

    Task<WorkoutDayResponse?> GroupExercisesInSupersetAsync(Guid workoutDayId, GroupSupersetRequest request);

    Task<WorkoutDayResponse?> UngroupExercisesFromSupersetAsync(Guid supersetGroupId);

    // Set operations - return WorkoutExerciseResponse
    Task<WorkoutExerciseResponse?> AddSetAsync(Guid workoutExerciseId, CreateExerciseSetRequest request);
    Task<WorkoutExerciseResponse?> UpdateSetAsync(Guid setId, UpdateExerciseSetRequest request);
    Task<WorkoutExerciseResponse?> CompleteSetAsync(Guid setId, CompleteSetRequest request);
    Task<WorkoutExerciseResponse?> DeleteSetAsync(Guid setId);
    Task<WorkoutExerciseResponse?> CreateDropSetSequenceAsync(Guid workoutExerciseId, CreateDropSetRequest request);
}