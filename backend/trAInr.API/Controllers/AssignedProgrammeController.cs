using Microsoft.AspNetCore.Mvc;
using trAInr.Application.DTOs;
using trAInr.Application.Interfaces.Services;

namespace trAInr.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssignedProgrammeController(IAssignedProgrammeService assignedProgramService, IAthleteService athleteService) : ControllerBase
{
    /// <summary>
    ///     Get all pre-made programmes
    /// </summary>
    [HttpGet("premade")]
    public async Task<ActionResult<IEnumerable<ProgrammeSummaryResponse>>> GetPreMade()
    {
        var programmes = await assignedProgramService.GetPreMadeProgrammesAsync();
        return Ok(programmes);
    }

    /// <summary>
    ///     Get all assigned programmes for an athlete
    /// </summary>
    [HttpGet("athlete/{athleteId:guid}")]
    public async Task<ActionResult<IEnumerable<ProgrammeSummaryResponse>>> GetByAthlete(Guid athleteId)
    {
        if (!await athleteService.ExistsAsync(athleteId)) return NotFound("Athlete not found");

        var programmes = await assignedProgramService.GetByAthleteIdAsync(athleteId);
        return Ok(programmes);
    }

    /// <summary>
    ///     Get the active assigned programme for an athlete
    /// </summary>
    [HttpGet("athlete/{athleteId:guid}/active")]
    public async Task<ActionResult<ProgrammeSummaryResponse>> GetActiveByAthlete(Guid athleteId)
    {
        var programme = await assignedProgramService.GetActiveByAthleteIdAsync(athleteId);
        if (programme is null) return NotFound("No active programme found");
        return Ok(programme);
    }

    /// <summary>
    ///     Get assigned programme by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProgrammeResponse>> GetById(Guid id)
    {
        var programme = await assignedProgramService.GetByIdAsync(id);
        if (programme is null) return NotFound();
        return Ok(programme);
    }

    /// <summary>
    ///     Create a new assigned programme for an athlete
    /// </summary>
    [HttpPost("athlete/{athleteId:guid}")]
    public async Task<ActionResult<ProgrammeResponse>> Create(Guid athleteId, [FromBody] CreateProgrammeRequest request)
    {
        if (!await athleteService.ExistsAsync(athleteId)) return NotFound("Athlete not found");

        if (request.DurationWeeks < 4 || request.DurationWeeks > 10)
            return BadRequest("Programme duration must be between 4 and 10 weeks");

        var programme = await assignedProgramService.CreateAsync(athleteId, request);
        return CreatedAtAction(nameof(Create), new { id = programme.Id }, programme);
    }

    /// <summary>
    ///     Update an assigned programme
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProgrammeResponse>> Update(Guid id, [FromBody] UpdateProgrammeRequest request)
    {
        var programme = await assignedProgramService.UpdateAsync(id, request);
        if (programme is null) return NotFound();
        return Ok(programme);
    }

    /// <summary>
    ///     Delete an assigned programme
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var deleted = await assignedProgramService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    /// <summary>
    ///     Clone a pre-made programme for an athlete
    /// </summary>
    [HttpPost("{programmeId:guid}/clone/{athleteId:guid}")]
    public async Task<ActionResult<ProgrammeResponse>> CloneProgramme(Guid programmeId, Guid athleteId)
    {
        if (!await athleteService.ExistsAsync(athleteId)) return NotFound("Athlete not found");

        var programme = await assignedProgramService.CloneProgrammeAsync(programmeId, athleteId);
        if (programme is null) return NotFound("Programme not found");

        return CreatedAtAction(nameof(GetById), new { id = programme.Id }, programme);
    }

    /// <summary>
    ///     Add a week to an assigned programme
    /// </summary>
    [HttpPost("{programmeId:guid}/weeks")]
    public async Task<ActionResult<ProgrammeWeekResponse>> AddWeek(
        Guid programmeId,
        [FromBody] CreateProgrammeWeekRequest request)
    {
        var week = await assignedProgramService.AddWeekAsync(programmeId, request);
        if (week is null) return NotFound("Programme not found");
        return Ok(week);
    }

    /// <summary>
    ///     Update a programme week
    /// </summary>
    [HttpPut("weeks/{weekId:guid}")]
    public async Task<ActionResult<ProgrammeWeekResponse>> UpdateWeek(
        Guid weekId,
        [FromBody] UpdateProgrammeWeekRequest request)
    {
        var week = await assignedProgramService.UpdateWeekAsync(weekId, request);
        if (week is null) return NotFound();
        return Ok(week);
    }

    /// <summary>
    ///     Copy a week to a new week number with all exercises, sets, and configuration.
    ///     The copied week will have all completion statuses reset.
    /// </summary>
    [HttpPost("weeks/{weekId:guid}/copy")]
    public async Task<ActionResult<ProgrammeWeekResponse>> CopyWeek(
        Guid weekId,
        [FromBody] CopyWeekRequest request)
    {
        var week = await assignedProgramService.CopyWeekAsync(weekId, request.TargetWeekNumber);
        if (week is null) return BadRequest("Failed to copy week. Target week may already exist or source week not found.");
        return Ok(week);
    }

    /// <summary>
    ///     Copy exercises from a source week into an existing target week.
    ///     All completion statuses are reset. Existing content in target week is preserved.
    /// </summary>
    [HttpPost("weeks/{sourceWeekId:guid}/copy-to/{targetWeekId:guid}")]
    public async Task<ActionResult<ProgrammeWeekResponse>> CopyWeekContent(
        Guid sourceWeekId,
        Guid targetWeekId)
    {
        var week = await assignedProgramService.CopyWeekContentAsync(sourceWeekId, targetWeekId);
        if (week is null) return BadRequest("Failed to copy week content. Source or target week not found.");
        return Ok(week);
    }
}