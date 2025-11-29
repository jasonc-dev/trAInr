using Microsoft.AspNetCore.Mvc;
using trAInr.API.Models.DTOs;
using trAInr.API.Services;

namespace trAInr.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProgrammesController(IProgrammeService programmeService, IUserService userService) : ControllerBase
{
    private readonly IProgrammeService _programmeService = programmeService;
    private readonly IUserService _userService = userService;

    /// <summary>
    /// Get all pre-made programmes
    /// </summary>
    [HttpGet("premade")]
    public async Task<ActionResult<IEnumerable<ProgrammeSummaryResponse>>> GetPreMade()
    {
        var programmes = await _programmeService.GetPreMadeProgrammesAsync();
        return Ok(programmes);
    }

    /// <summary>
    /// Get all programmes for a user
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<IEnumerable<ProgrammeSummaryResponse>>> GetByUser(Guid userId)
    {
        if (!await _userService.ExistsAsync(userId))
        {
            return NotFound("User not found");
        }

        var programmes = await _programmeService.GetByUserIdAsync(userId);
        return Ok(programmes);
    }

    /// <summary>
    /// Get the active programme for a user
    /// </summary>
    [HttpGet("user/{userId:guid}/active")]
    public async Task<ActionResult<ProgrammeSummaryResponse>> GetActiveByUser(Guid userId)
    {
        var programme = await _programmeService.GetActiveByUserIdAsync(userId);
        if (programme is null) return NotFound("No active programme found");
        return Ok(programme);
    }

    /// <summary>
    /// Get programme by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProgrammeResponse>> GetById(Guid id)
    {
        var programme = await _programmeService.GetByIdAsync(id);
        if (programme is null) return NotFound();
        return Ok(programme);
    }

    /// <summary>
    /// Create a new programme for a user
    /// </summary>
    [HttpPost("user/{userId:guid}")]
    public async Task<ActionResult<ProgrammeResponse>> Create(Guid userId, [FromBody] CreateProgrammeRequest request)
    {
        if (!await _userService.ExistsAsync(userId))
        {
            return NotFound("User not found");
        }

        if (request.DurationWeeks < 4 || request.DurationWeeks > 10)
        {
            return BadRequest("Programme duration must be between 4 and 10 weeks");
        }

        var programme = await _programmeService.CreateAsync(userId, request);
        return CreatedAtAction(nameof(GetById), new { id = programme.Id }, programme);
    }

    /// <summary>
    /// Update a programme
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProgrammeResponse>> Update(Guid id, [FromBody] UpdateProgrammeRequest request)
    {
        var programme = await _programmeService.UpdateAsync(id, request);
        if (programme is null) return NotFound();
        return Ok(programme);
    }

    /// <summary>
    /// Delete a programme
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var deleted = await _programmeService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Clone a pre-made programme for a user
    /// </summary>
    [HttpPost("{programmeId:guid}/clone/{userId:guid}")]
    public async Task<ActionResult<ProgrammeResponse>> CloneProgramme(Guid programmeId, Guid userId)
    {
        if (!await _userService.ExistsAsync(userId))
        {
            return NotFound("User not found");
        }

        var programme = await _programmeService.CloneProgrammeAsync(programmeId, userId);
        if (programme is null) return NotFound("Programme not found");

        return CreatedAtAction(nameof(GetById), new { id = programme.Id }, programme);
    }

    /// <summary>
    /// Add a week to a programme
    /// </summary>
    [HttpPost("{programmeId:guid}/weeks")]
    public async Task<ActionResult<ProgrammeWeekResponse>> AddWeek(
        Guid programmeId,
        [FromBody] CreateProgrammeWeekRequest request)
    {
        var week = await _programmeService.AddWeekAsync(programmeId, request);
        if (week is null) return NotFound("Programme not found");
        return Ok(week);
    }

    /// <summary>
    /// Update a week
    /// </summary>
    [HttpPut("weeks/{weekId:guid}")]
    public async Task<ActionResult<ProgrammeWeekResponse>> UpdateWeek(
        Guid weekId,
        [FromBody] UpdateProgrammeWeekRequest request)
    {
        var week = await _programmeService.UpdateWeekAsync(weekId, request);
        if (week is null) return NotFound();
        return Ok(week);
    }
}

