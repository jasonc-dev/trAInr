namespace trAInr.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using trAInr.Application.DTOs.AI;
using trAInr.Application.DTOs.ProgramTemplate;
using trAInr.Application.Interfaces.Services.AI;
using trAInr.Domain.Aggregates;

[ApiController]
[Route("api/[controller]")]
public class ProgramGeneratorController(IAiProgramGeneratorService aiProgramGeneratorService, IOpenAiClient openAiClient)
    : ControllerBase
{
  [HttpPost]
  [ProducesResponseType(typeof(ProgramTemplate), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<ProgramTemplate>> GenerateProgram([FromBody] GenerateProgamRequest request, CancellationToken cancellationToken = default)
  {
    var program = await aiProgramGeneratorService.GenerateProgramAsync(request, cancellationToken);
    return Ok(program);
  }

  [HttpGet("test")]
  public async Task<ActionResult<ProgramTemplateResponse>> Test(CancellationToken cancellationToken = default)
  {
    var response = await openAiClient.GenerateTestResponse(cancellationToken);
    return Ok(response);
  }
}