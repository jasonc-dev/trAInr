using Microsoft.AspNetCore.Mvc;
using trAInr.API.Models.DTOs;
using trAInr.API.Services;

namespace trAInr.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    /// <summary>
    /// Get all users
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserSummaryResponse>>> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponse>> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user is null) return NotFound();
        return Ok(user);
    }

    /// <summary>
    /// Get user by email
    /// </summary>
    [HttpGet("email/{email}")]
    public async Task<ActionResult<UserResponse>> GetByEmail(string email)
    {
        var user = await _userService.GetByEmailAsync(email);
        if (user is null) return NotFound();
        return Ok(user);
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create([FromBody] CreateUserRequest request)
    {
        var existingUser = await _userService.GetByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            return Conflict("A user with this email already exists");
        }

        var user = await _userService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserResponse>> Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        var user = await _userService.UpdateAsync(id, request);
        if (user is null) return NotFound();
        return Ok(user);
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var deleted = await _userService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}

