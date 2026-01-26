using trAInr.Domain.Entities;

namespace trAInr.Application.Interfaces.Repositories;

public interface IJobRepository
{
    Task<AiGenerationJob?> GetByIdAsync(Guid id);
    Task<AiGenerationJob?> GetNextPendingJobAsync();
    Task AddAsync(AiGenerationJob job);
    void Update(AiGenerationJob job);
}
