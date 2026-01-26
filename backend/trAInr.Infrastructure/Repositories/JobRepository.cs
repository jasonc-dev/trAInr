using Microsoft.EntityFrameworkCore;
using trAInr.Application.Interfaces.Repositories;
using trAInr.Domain.Entities;
using trAInr.Infrastructure.Data;

namespace trAInr.Infrastructure.Repositories;

public class JobRepository(TrainrDbContext context) : IJobRepository
{
    public async Task<AiGenerationJob?> GetByIdAsync(Guid id)
    {
        return await context.AiGenerationJobs.FindAsync(id);
    }

    public async Task<AiGenerationJob?> GetNextPendingJobAsync()
    {
        return await context.AiGenerationJobs
            .Where(j => j.Status == "Pending")
            .OrderBy(j => j.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(AiGenerationJob job)
    {
        await context.AiGenerationJobs.AddAsync(job);
    }

    public void Update(AiGenerationJob job)
    {
        context.AiGenerationJobs.Update(job);
    }
}
