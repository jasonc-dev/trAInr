using trAInr.Application.DTOs;

namespace trAInr.Application.Interfaces.Services;

public interface IWorkoutSessionService
{
    Task<WorkoutDayResponse?> GetWorkoutDayAsync(Guid workoutDayId);

    Task<WorkoutDayResponse?> CreateWorkoutDayAsync(Guid weekId, CreateWorkoutDayRequest request);

    Task<WorkoutDayResponse?> UpdateWorkoutDayAsync(Guid workoutDayId, UpdateWorkoutDayRequest request);

    Task<bool> DeleteWorkoutDayAsync(Guid workoutDayId);

    Task<WorkoutDayResponse?> CompleteWorkoutAsync(Guid workoutDayId, CompleteWorkoutRequest request);

    Task<WorkoutExerciseResponse?> AddExerciseToWorkoutAsync(Guid workoutDayId, AddWorkoutExerciseRequest request);

    Task<WorkoutExerciseResponse?> UpdateWorkoutExerciseAsync(Guid workoutExerciseId,
        UpdateWorkoutExerciseRequest request);

    Task<bool> RemoveExerciseFromWorkoutAsync(Guid workoutExerciseId);
    
    Task<bool> ReorderExercisesAsync(Guid workoutDayId, List<Guid> exerciseIds);

    Task<ExerciseSetResponse?> AddSetAsync(Guid workoutExerciseId, CreateExerciseSetRequest request);
    Task<ExerciseSetResponse?> UpdateSetAsync(Guid setId, UpdateExerciseSetRequest request);
    Task<ExerciseSetResponse?> CompleteSetAsync(Guid setId, CompleteSetRequest request);
    Task<bool> DeleteSetAsync(Guid setId);
    
    // Superset and Drop Set Operations
    Task<IEnumerable<WorkoutExerciseResponse>?> GroupExercisesInSupersetAsync(Guid workoutDayId, GroupSupersetRequest request);
    Task<bool> UngroupExercisesFromSupersetAsync(Guid supersetGroupId);
    Task<IEnumerable<ExerciseSetResponse>?> CreateDropSetSequenceAsync(Guid workoutExerciseId, CreateDropSetRequest request);
}