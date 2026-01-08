using Microsoft.AspNetCore.Mvc;
using trAInr.Application.DTOs;
using trAInr.Application.Interfaces.Services;

namespace trAInr.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AthleteController(IAthleteService athleteService) : ControllerBase
{
    /// <summary>
    ///     Get all athletes
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserSummaryResponse>>> GetAll()
    {
        var athletes = await athleteService.GetAllAsync();
        return Ok(athletes);
    }

    /// <summary>
    ///     Get athlete by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponse>> GetById(Guid id)
    {
        var athlete = await athleteService.GetByIdAsync(id);
        if (athlete is null) return NotFound();
        return Ok(athlete);
    }

    /// <summary>
    ///     Get athlete by email
    /// </summary>
    [HttpGet("email/{email}")]
    public async Task<ActionResult<UserResponse>> GetByEmail(string email)
    {
        var athlete = await athleteService.GetByEmailAsync(email);
        if (athlete is null) return NotFound();
        return Ok(athlete);
    }

    /// <summary>
    ///     Create a new athlete
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create([FromBody] CreateUserRequest request)
    {
        var existingAthlete = await athleteService.GetByEmailAsync(request.Email);
        if (existingAthlete is not null) return Conflict("An athlete with this email already exists");

        var athlete = await athleteService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = athlete.Id }, athlete);
    }

    /// <summary>
    ///     Update an existing athlete
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserResponse>> Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        var athlete = await athleteService.UpdateAsync(id, request);
        if (athlete is null) return NotFound();
        return Ok(athlete);
    }

    /// <summary>
    ///     Delete an athlete
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var deleted = await athleteService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}