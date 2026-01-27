using trAInr.Application.DTOs;

namespace trAInr.Application.Interfaces.Services;

public interface IAssignedProgrammeService
{
    Task<ProgrammeResponse?> GetByIdAsync(Guid id);
    Task<IEnumerable<ProgrammeSummaryResponse>> GetByAthleteIdAsync(Guid athleteId);
    Task<ProgrammeSummaryResponse?> GetActiveByAthleteIdAsync(Guid athleteId);
    Task<ProgrammeResponse> CreateAsync(Guid athleteId, CreateProgrammeRequest request);
    Task<ProgrammeResponse?> UpdateAsync(Guid id, UpdateProgrammeRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<ProgrammeWeekResponse?> AddWeekAsync(Guid programmeId, CreateProgrammeWeekRequest request);
    Task<ProgrammeWeekResponse?> UpdateWeekAsync(Guid weekId, UpdateProgrammeWeekRequest request);
    Task<IEnumerable<ProgrammeSummaryResponse>> GetPreMadeProgrammesAsync();
    Task<IEnumerable<ProgrammeSummaryResponse>> GetProgrammesCreatedByAthleteAsync(Guid athleteId);
    Task<ProgrammeResponse?> CloneProgrammeAsync(Guid programmeId, CloneProgrammeRequest request);
    Task<ProgrammeWeekResponse?> CopyWeekAsync(Guid sourceWeekId, int targetWeekNumber);
    Task<ProgrammeWeekResponse?> CopyWeekContentAsync(Guid sourceWeekId, Guid targetWeekId);
}