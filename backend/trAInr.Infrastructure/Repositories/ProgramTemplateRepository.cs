using Microsoft.EntityFrameworkCore;
using trAInr.Application.Interfaces.Repositories;
using trAInr.Domain.Aggregates;
using trAInr.Infrastructure.Data;

namespace trAInr.Infrastructure.Repositories;

public class ProgramTemplateRepository(TrainrDbContext context) : IProgramTemplateRepository
{
    public async Task<ProgramTemplate?> GetByIdAsync(Guid id)
    {
        return await context.ProgramTemplates
            .Include(pt => pt.Weeks)
                .ThenInclude(w => w.WorkoutDays)
                    .ThenInclude(wd => wd.Exercises)
                        .ThenInclude(we => we.ExerciseDefinition)
            .FirstOrDefaultAsync(pt => pt.Id == id);
    }

    public async Task<IEnumerable<ProgramTemplate>> GetAllActiveAsync()
    {
        return await context.ProgramTemplates
            .Where(pt => pt.IsActive && !pt.IsUserGenerated)
            .Include(pt => pt.Weeks)
                .ThenInclude(w => w.WorkoutDays)
                    .ThenInclude(wd => wd.Exercises)
                        .ThenInclude(we => we.ExerciseDefinition)
                        .AsSplitQuery()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<ProgramTemplate>> GetProgrammesCreatedByAthleteAsync(Guid athleteId)
    {
        return await context.ProgramTemplates
            .Where(pt => pt.IsActive && pt.IsUserGenerated && pt.CreatedBy == athleteId)
            .Include(pt => pt.Weeks)
                .ThenInclude(w => w.WorkoutDays)
                    .ThenInclude(wd => wd.Exercises)
                        .ThenInclude(we => we.ExerciseDefinition)
                        .AsSplitQuery()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(ProgramTemplate programTemplate)
    {
        await context.ProgramTemplates.AddAsync(programTemplate);
    }

    public void Update(ProgramTemplate programTemplate)
    {
        context.ProgramTemplates.Update(programTemplate);
    }

    public void Delete(ProgramTemplate programTemplate)
    {
        context.ProgramTemplates.Remove(programTemplate);
    }
}