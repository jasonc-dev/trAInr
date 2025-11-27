using Microsoft.EntityFrameworkCore;
using trAInr.API.Data;
using trAInr.API.Models.Domain;
using trAInr.API.Models.DTOs;

namespace trAInr.API.Services;

public class ProgrammeService : IProgrammeService
{
    private readonly TrainrDbContext _context;

    public ProgrammeService(TrainrDbContext context)
    {
        _context = context;
    }

    public async Task<ProgrammeResponse?> GetByIdAsync(Guid id)
    {
        var programme = await _context.Programmes
            .Include(p => p.Weeks)
                .ThenInclude(w => w.WorkoutDays)
                    .ThenInclude(d => d.Exercises)
                        .ThenInclude(e => e.Exercise)
            .Include(p => p.Weeks)
                .ThenInclude(w => w.WorkoutDays)
                    .ThenInclude(d => d.Exercises)
                        .ThenInclude(e => e.Sets)
            .FirstOrDefaultAsync(p => p.Id == id);

        return programme is null ? null : MapToResponse(programme);
    }

    public async Task<IEnumerable<ProgrammeSummaryResponse>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Programmes
            .Where(p => p.UserId == userId)
            .Include(p => p.Weeks)
            .Select(p => MapToSummary(p))
            .ToListAsync();
    }

    public async Task<ProgrammeSummaryResponse?> GetActiveByUserIdAsync(Guid userId)
    {
        var programme = await _context.Programmes
            .Where(p => p.UserId == userId && p.IsActive)
            .Include(p => p.Weeks)
            .FirstOrDefaultAsync();

        return programme is null ? null : MapToSummary(programme);
    }

    public async Task<ProgrammeResponse> CreateAsync(Guid userId, CreateProgrammeRequest request)
    {
        // Deactivate any existing active programme
        var existingActive = await _context.Programmes
            .Where(p => p.UserId == userId && p.IsActive)
            .ToListAsync();

        foreach (var p in existingActive)
        {
            p.IsActive = false;
        }

        var programme = new Programme
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = request.Name,
            Description = request.Description,
            DurationWeeks = request.DurationWeeks,
            IsPreMade = false,
            IsActive = true,
            StartDate = request.StartDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Create weeks for the programme
        for (int i = 1; i <= request.DurationWeeks; i++)
        {
            programme.Weeks.Add(new ProgrammeWeek
            {
                Id = Guid.NewGuid(),
                ProgrammeId = programme.Id,
                WeekNumber = i,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            });
        }

        _context.Programmes.Add(programme);
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(programme.Id))!;
    }

    public async Task<ProgrammeResponse?> UpdateAsync(Guid id, UpdateProgrammeRequest request)
    {
        var programme = await _context.Programmes.FindAsync(id);
        if (programme is null) return null;

        // If activating this programme, deactivate others
        if (request.IsActive && !programme.IsActive)
        {
            var otherActive = await _context.Programmes
                .Where(p => p.UserId == programme.UserId && p.IsActive && p.Id != id)
                .ToListAsync();

            foreach (var p in otherActive)
            {
                p.IsActive = false;
            }
        }

        programme.Name = request.Name;
        programme.Description = request.Description;
        programme.IsActive = request.IsActive;
        programme.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var programme = await _context.Programmes.FindAsync(id);
        if (programme is null) return false;

        _context.Programmes.Remove(programme);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ProgrammeWeekResponse?> AddWeekAsync(Guid programmeId, CreateProgrammeWeekRequest request)
    {
        var programme = await _context.Programmes
            .Include(p => p.Weeks)
            .FirstOrDefaultAsync(p => p.Id == programmeId);

        if (programme is null) return null;

        var week = new ProgrammeWeek
        {
            Id = Guid.NewGuid(),
            ProgrammeId = programmeId,
            WeekNumber = request.WeekNumber,
            Notes = request.Notes,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        programme.Weeks.Add(week);
        programme.DurationWeeks = programme.Weeks.Count;
        await _context.SaveChangesAsync();

        return MapToWeekResponse(week);
    }

    public async Task<ProgrammeWeekResponse?> UpdateWeekAsync(Guid weekId, UpdateProgrammeWeekRequest request)
    {
        var week = await _context.ProgrammeWeeks
            .Include(w => w.WorkoutDays)
                .ThenInclude(d => d.Exercises)
                    .ThenInclude(e => e.Exercise)
            .Include(w => w.WorkoutDays)
                .ThenInclude(d => d.Exercises)
                    .ThenInclude(e => e.Sets)
            .FirstOrDefaultAsync(w => w.Id == weekId);

        if (week is null) return null;

        week.Notes = request.Notes;
        week.IsCompleted = request.IsCompleted;

        await _context.SaveChangesAsync();
        return MapToWeekResponse(week);
    }

    public async Task<IEnumerable<ProgrammeSummaryResponse>> GetPreMadeProgrammesAsync()
    {
        return await _context.Programmes
            .Where(p => p.IsPreMade)
            .Include(p => p.Weeks)
            .Select(p => MapToSummary(p))
            .ToListAsync();
    }

    public async Task<ProgrammeResponse?> CloneProgrammeAsync(Guid programmeId, Guid userId)
    {
        var source = await _context.Programmes
            .Include(p => p.Weeks)
                .ThenInclude(w => w.WorkoutDays)
                    .ThenInclude(d => d.Exercises)
            .FirstOrDefaultAsync(p => p.Id == programmeId);

        if (source is null) return null;

        // Deactivate existing active programmes
        var existingActive = await _context.Programmes
            .Where(p => p.UserId == userId && p.IsActive)
            .ToListAsync();

        foreach (var p in existingActive)
        {
            p.IsActive = false;
        }

        var clone = new Programme
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = source.Name,
            Description = source.Description,
            DurationWeeks = source.DurationWeeks,
            IsPreMade = false,
            IsActive = true,
            StartDate = DateTime.UtcNow.Date,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        foreach (var sourceWeek in source.Weeks.OrderBy(w => w.WeekNumber))
        {
            var cloneWeek = new ProgrammeWeek
            {
                Id = Guid.NewGuid(),
                ProgrammeId = clone.Id,
                WeekNumber = sourceWeek.WeekNumber,
                Notes = sourceWeek.Notes,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var sourceDay in sourceWeek.WorkoutDays.OrderBy(d => d.DayOfWeek))
            {
                var cloneDay = new WorkoutDay
                {
                    Id = Guid.NewGuid(),
                    ProgrammeWeekId = cloneWeek.Id,
                    DayOfWeek = sourceDay.DayOfWeek,
                    Name = sourceDay.Name,
                    Description = sourceDay.Description,
                    IsRestDay = sourceDay.IsRestDay,
                    IsCompleted = false,
                    CreatedAt = DateTime.UtcNow
                };

                foreach (var sourceExercise in sourceDay.Exercises.OrderBy(e => e.OrderIndex))
                {
                    cloneDay.Exercises.Add(new WorkoutExercise
                    {
                        Id = Guid.NewGuid(),
                        WorkoutDayId = cloneDay.Id,
                        ExerciseId = sourceExercise.ExerciseId,
                        OrderIndex = sourceExercise.OrderIndex,
                        Notes = sourceExercise.Notes,
                        TargetSets = sourceExercise.TargetSets,
                        TargetReps = sourceExercise.TargetReps,
                        TargetWeight = sourceExercise.TargetWeight,
                        TargetDurationSeconds = sourceExercise.TargetDurationSeconds,
                        TargetDistance = sourceExercise.TargetDistance,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                cloneWeek.WorkoutDays.Add(cloneDay);
            }

            clone.Weeks.Add(cloneWeek);
        }

        _context.Programmes.Add(clone);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(clone.Id);
    }

    private static ProgrammeSummaryResponse MapToSummary(Programme p)
    {
        var completedWeeks = p.Weeks.Count(w => w.IsCompleted);
        var totalWeeks = p.Weeks.Count > 0 ? p.Weeks.Count : p.DurationWeeks;
        var progress = totalWeeks > 0 ? (double)completedWeeks / totalWeeks * 100 : 0;

        return new ProgrammeSummaryResponse(
            p.Id,
            p.Name,
            p.Description,
            p.DurationWeeks,
            p.IsActive,
            p.StartDate,
            completedWeeks,
            Math.Round(progress, 1));
    }

    private static ProgrammeResponse MapToResponse(Programme p) =>
        new(
            p.Id,
            p.UserId,
            p.Name,
            p.Description,
            p.DurationWeeks,
            p.IsPreMade,
            p.IsActive,
            p.StartDate,
            p.EndDate,
            p.CreatedAt,
            p.Weeks.OrderBy(w => w.WeekNumber).Select(MapToWeekResponse));

    private static ProgrammeWeekResponse MapToWeekResponse(ProgrammeWeek w) =>
        new(
            w.Id,
            w.WeekNumber,
            w.Notes,
            w.IsCompleted,
            w.WorkoutDays.OrderBy(d => d.DayOfWeek).Select(MapToWorkoutDayResponse));

    private static WorkoutDayResponse MapToWorkoutDayResponse(WorkoutDay d) =>
        new(
            d.Id,
            d.ProgrammeWeekId,
            d.DayOfWeek,
            d.Name,
            d.Description,
            d.ScheduledDate,
            d.CompletedDate,
            d.IsCompleted,
            d.IsRestDay,
            d.Exercises.OrderBy(e => e.OrderIndex).Select(MapToWorkoutExerciseResponse));

    private static WorkoutExerciseResponse MapToWorkoutExerciseResponse(WorkoutExercise e) =>
        new(
            e.Id,
            e.ExerciseId,
            e.Exercise?.Name ?? "Unknown",
            e.OrderIndex,
            e.Notes,
            e.TargetSets,
            e.TargetReps,
            e.TargetWeight,
            e.TargetDurationSeconds,
            e.TargetDistance,
            e.Sets.OrderBy(s => s.SetNumber).Select(MapToExerciseSetResponse));

    private static ExerciseSetResponse MapToExerciseSetResponse(ExerciseSet s) =>
        new(
            s.Id,
            s.SetNumber,
            s.Reps,
            s.Weight,
            s.DurationSeconds,
            s.Distance,
            s.Difficulty,
            s.Intensity,
            s.IsCompleted,
            s.IsWarmup,
            s.Notes,
            s.CompletedAt);
}

