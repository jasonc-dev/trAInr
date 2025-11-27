using trAInr.API.Models.DTOs;

namespace trAInr.API.Services;

public interface IProgrammeService
{
    Task<ProgrammeResponse?> GetByIdAsync(Guid id);
    Task<IEnumerable<ProgrammeSummaryResponse>> GetByUserIdAsync(Guid userId);
    Task<ProgrammeSummaryResponse?> GetActiveByUserIdAsync(Guid userId);
    Task<ProgrammeResponse> CreateAsync(Guid userId, CreateProgrammeRequest request);
    Task<ProgrammeResponse?> UpdateAsync(Guid id, UpdateProgrammeRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<ProgrammeWeekResponse?> AddWeekAsync(Guid programmeId, CreateProgrammeWeekRequest request);
    Task<ProgrammeWeekResponse?> UpdateWeekAsync(Guid weekId, UpdateProgrammeWeekRequest request);
    Task<IEnumerable<ProgrammeSummaryResponse>> GetPreMadeProgrammesAsync();
    Task<ProgrammeResponse?> CloneProgrammeAsync(Guid programmeId, Guid userId);
}

