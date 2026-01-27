using trAInr.Domain.Aggregates;

namespace trAInr.Application.Interfaces.Repositories;

public interface IProgramTemplateRepository
{
    Task<ProgramTemplate?> GetByIdAsync(Guid id);
    Task<IEnumerable<ProgramTemplate>> GetAllActiveAsync();
    Task<IEnumerable<ProgramTemplate>> GetProgrammesCreatedByAthleteAsync(Guid athleteId);
    Task AddAsync(ProgramTemplate programTemplate);
    void Update(ProgramTemplate programTemplate);
    void Delete(ProgramTemplate programTemplate);
}