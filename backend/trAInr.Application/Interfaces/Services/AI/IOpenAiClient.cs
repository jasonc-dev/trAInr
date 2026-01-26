namespace trAInr.Application.Interfaces.Services.AI;

public interface IOpenAiClient
{
  Task<string> GenerateProgramTemplate(string prompt, CancellationToken ct = default);
  Task<string> GenerateTestResponse(CancellationToken ct = default);
}