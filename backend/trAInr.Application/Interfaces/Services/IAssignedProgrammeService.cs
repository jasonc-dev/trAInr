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
    Task<ProgrammeResponse?> CloneProgrammeAsync(Guid programmeId, Guid userId);
    
    /// <summary>
    ///     Copies a week's workout days, exercises, and sets to a new week.
    ///     All completion statuses are reset to false on the copied week.
    /// </summary>
    Task<ProgrammeWeekResponse?> CopyWeekAsync(Guid sourceWeekId, int targetWeekNumber);
    
    /// <summary>
    ///     Copies workout days, exercises, and sets from a source week into an existing target week.
    ///     All completion statuses are reset to false. Any existing content in the target week is preserved.
    /// </summary>
    Task<ProgrammeWeekResponse?> CopyWeekContentAsync(Guid sourceWeekId, Guid targetWeekId);
}