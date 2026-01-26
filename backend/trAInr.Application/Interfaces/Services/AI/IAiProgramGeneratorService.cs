using trAInr.Application.DTOs.AI;
using trAInr.Application.DTOs.ProgramTemplate;

namespace trAInr.Application.Interfaces.Services.AI;

public interface IAiProgramGeneratorService
{
  Task<ProgramTemplateResponse> GenerateProgramAsync(GenerateProgamRequest request, CancellationToken cancellationToken = default);
}