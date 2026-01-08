using Microsoft.EntityFrameworkCore;
using trAInr.Application.Interfaces.Repositories;
using trAInr.Domain.Aggregates;
using trAInr.Domain.Entities;
using trAInr.Infrastructure.Data;

namespace trAInr.Infrastructure.Repositories;

/// <summary>
///     Repository implementation for AssignedProgram aggregate root.
/// </summary>
public class AssignedProgramRepository(TrainrDbContext context) : IAssignedProgramRepository
{
    public async Task<AssignedProgram?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.AssignedPrograms
            .Include(p => p.Weeks)
            .ThenInclude(w => w.WorkoutDays)
            .ThenInclude(d => d.Exercises)
            .ThenInclude(e => e.ExerciseDefinition)
            .Include(p => p.Weeks)
            .ThenInclude(w => w.WorkoutDays)
            .ThenInclude(d => d.Exercises)
            .ThenInclude(e => e.Sets)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<AssignedProgram>> GetByAthleteIdAsync(Guid athleteId,
        CancellationToken cancellationToken = default)
    {
        return await context.AssignedPrograms
            .Include(p => p.Weeks)
            .Where(p => p.AthleteId == athleteId)
            .OrderByDescending(p => p.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<AssignedProgram?> GetActiveByAthleteIdAsync(Guid athleteId,
        CancellationToken cancellationToken = default)
    {
        return await context.AssignedPrograms
            .Include(p => p.Weeks)
            .Where(p => p.AthleteId == athleteId && p.IsActive)
            .OrderByDescending(p => p.StartDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<AssignedProgram?> GetByWeekIdAsync(Guid weekId, CancellationToken cancellationToken = default)
    {
        return await context.AssignedPrograms
            .Include(p => p.Weeks)
            .ThenInclude(w => w.WorkoutDays)
            .ThenInclude(d => d.Exercises)
            .ThenInclude(e => e.ExerciseDefinition)
            .Include(p => p.Weeks)
            .ThenInclude(w => w.WorkoutDays)
            .ThenInclude(d => d.Exercises)
            .ThenInclude(e => e.Sets)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.Weeks.Any(w => w.Id == weekId), cancellationToken);
    }

    public async Task<AssignedProgram?> GetByWorkoutDayIdAsync(Guid workoutDayId, CancellationToken cancellationToken = default)
    {
        return await context.AssignedPrograms
            .Include(p => p.Weeks)
            .ThenInclude(w => w.WorkoutDays)
            .ThenInclude(d => d.Exercises)
            .ThenInclude(e => e.ExerciseDefinition)
            .Include(p => p.Weeks)
            .ThenInclude(w => w.WorkoutDays)
            .ThenInclude(d => d.Exercises)
            .ThenInclude(e => e.Sets)
            .FirstOrDefaultAsync(p => p.Weeks.Any(w => w.WorkoutDays.Any(d => d.Id == workoutDayId)), cancellationToken);
    }

    public async Task<AssignedProgram?> GetByWorkoutExerciseIdAsync(Guid workoutExerciseId, CancellationToken cancellationToken = default)
    {
        return await context.AssignedPrograms
            .Include(p => p.Weeks)
            .ThenInclude(w => w.WorkoutDays)
            .ThenInclude(d => d.Exercises)
            .ThenInclude(e => e.ExerciseDefinition)
            .Include(p => p.Weeks)
            .ThenInclude(w => w.WorkoutDays)
            .ThenInclude(d => d.Exercises)
            .ThenInclude(e => e.Sets)
            .FirstOrDefaultAsync(p => p.Weeks.Any(w => w.WorkoutDays.Any(d => d.Exercises.Any(e => e.Id == workoutExerciseId))), cancellationToken);
    }

    public async Task<AssignedProgram?> GetByExerciseSetIdAsync(Guid exerciseSetId, CancellationToken cancellationToken = default)
    {
        return await context.AssignedPrograms
            .Include(p => p.Weeks)
            .ThenInclude(w => w.WorkoutDays)
            .ThenInclude(d => d.Exercises)
            .ThenInclude(e => e.ExerciseDefinition)
            .Include(p => p.Weeks)
            .ThenInclude(w => w.WorkoutDays)
            .ThenInclude(d => d.Exercises)
            .ThenInclude(e => e.Sets)
            .FirstOrDefaultAsync(p => p.Weeks.Any(w => w.WorkoutDays.Any(d => d.Exercises.Any(e => e.Sets.Any(s => s.Id == exerciseSetId)))), cancellationToken);
    }

    public async Task<AssignedProgram?> GetBySupersetGroupIdAsync(Guid supersetGroupId, CancellationToken cancellationToken = default)
    {
        return await context.AssignedPrograms
            .Include(p => p.Weeks)
            .ThenInclude(w => w.WorkoutDays)
            .ThenInclude(d => d.Exercises)
            .ThenInclude(e => e.ExerciseDefinition)
            .Include(p => p.Weeks)
            .ThenInclude(w => w.WorkoutDays)
            .ThenInclude(d => d.Exercises)
            .ThenInclude(e => e.Sets)
            .FirstOrDefaultAsync(p => p.Weeks.Any(w => w.WorkoutDays.Any(d => d.Exercises.Any(e => e.SupersetGroupId == supersetGroupId))), cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.AssignedPrograms
            .AnyAsync(p => p.Id == id, cancellationToken);
    }

    public async Task AddAsync(AssignedProgram assignedProgram, CancellationToken cancellationToken = default)
    {
        try
        {
            await context.AssignedPrograms.AddAsync(assignedProgram, cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task UpdateAsync(AssignedProgram assignedProgram, CancellationToken cancellationToken = default)
    { 
        context.AssignedPrograms.Update(assignedProgram);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(AssignedProgram assignedProgram, CancellationToken cancellationToken = default)
    {
        context.AssignedPrograms.Remove(assignedProgram);
        await Task.CompletedTask;
    }

    public async Task AddProgrammeWeekAsync(ProgrammeWeek programmeWeek, CancellationToken cancellationToken = default)
    {
        await context.ProgrammeWeeks.AddAsync(programmeWeek, cancellationToken);
    }

    public async Task AddWorkoutDayAsync(WorkoutDay workoutDay, CancellationToken cancellationToken = default)
    {
        await context.WorkoutDays.AddAsync(workoutDay, cancellationToken);
    }

    public async Task AddWorkoutExerciseAsync(WorkoutExercise workoutExercise, CancellationToken cancellationToken = default)
    {
        await context.WorkoutExercises.AddAsync(workoutExercise, cancellationToken);
    }

    public async Task AddExerciseSetAsync(ExerciseSet exerciseSet, CancellationToken cancellationToken = default)
    {
        await context.ExerciseSets.AddAsync(exerciseSet, cancellationToken);
    }
}