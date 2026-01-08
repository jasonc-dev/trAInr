using trAInr.Domain.Aggregates;
using trAInr.Domain.Entities;

namespace trAInr.Application.Interfaces.Repositories;

/// <summary>
///     Repository interface for AssignedProgram aggregate root.
/// </summary>
public interface IAssignedProgramRepository
{
    Task<AssignedProgram?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<AssignedProgram>> GetByAthleteIdAsync(Guid athleteId, CancellationToken cancellationToken = default);
    Task<AssignedProgram?> GetActiveByAthleteIdAsync(Guid athleteId, CancellationToken cancellationToken = default);
    Task<AssignedProgram?> GetByWeekIdAsync(Guid weekId, CancellationToken cancellationToken = default);
    Task<AssignedProgram?> GetByWorkoutDayIdAsync(Guid workoutDayId, CancellationToken cancellationToken = default);
    Task<AssignedProgram?> GetByWorkoutExerciseIdAsync(Guid workoutExerciseId, CancellationToken cancellationToken = default);
    Task<AssignedProgram?> GetByExerciseSetIdAsync(Guid exerciseSetId, CancellationToken cancellationToken = default);
    Task<AssignedProgram?> GetBySupersetGroupIdAsync(Guid supersetGroupId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(AssignedProgram assignedProgram, CancellationToken cancellationToken = default);
    Task UpdateAsync(AssignedProgram assignedProgram, CancellationToken cancellationToken = default);
    Task DeleteAsync(AssignedProgram assignedProgram, CancellationToken cancellationToken = default);
    Task AddProgrammeWeekAsync(ProgrammeWeek programmeWeek, CancellationToken cancellationToken = default);
    Task AddWorkoutDayAsync(WorkoutDay workoutDay, CancellationToken cancellationToken = default);
    Task AddWorkoutExerciseAsync(WorkoutExercise workoutExercise, CancellationToken cancellationToken = default);
    Task AddExerciseSetAsync(ExerciseSet exerciseSet, CancellationToken cancellationToken = default);
}

