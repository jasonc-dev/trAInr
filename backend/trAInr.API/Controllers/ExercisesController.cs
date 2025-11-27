using Microsoft.AspNetCore.Mvc;
using trAInr.API.Models.Domain;
using trAInr.API.Models.DTOs;
using trAInr.API.Services;

namespace trAInr.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExercisesController : ControllerBase
{
    private readonly IExerciseService _exerciseService;

    public ExercisesController(IExerciseService exerciseService)
    {
        _exerciseService = exerciseService;
    }

    /// <summary>
    /// Get all exercises
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExerciseResponse>>> GetAll()
    {
        var exercises = await _exerciseService.GetAllAsync();
        return Ok(exercises);
    }

    /// <summary>
    /// Search exercises with optional filters
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ExerciseSummaryResponse>>> Search(
        [FromQuery] string? query = null,
        [FromQuery] ExerciseType? type = null,
        [FromQuery] MuscleGroup? muscleGroup = null)
    {
        var exercises = await _exerciseService.SearchAsync(query, type, muscleGroup);
        return Ok(exercises);
    }

    /// <summary>
    /// Get exercise by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ExerciseResponse>> GetById(Guid id)
    {
        var exercise = await _exerciseService.GetByIdAsync(id);
        if (exercise is null) return NotFound();
        return Ok(exercise);
    }

    /// <summary>
    /// Get exercises by type
    /// </summary>
    [HttpGet("type/{type}")]
    public async Task<ActionResult<IEnumerable<ExerciseSummaryResponse>>> GetByType(ExerciseType type)
    {
        var exercises = await _exerciseService.GetByTypeAsync(type);
        return Ok(exercises);
    }

    /// <summary>
    /// Get exercises by muscle group
    /// </summary>
    [HttpGet("muscle-group/{muscleGroup}")]
    public async Task<ActionResult<IEnumerable<ExerciseSummaryResponse>>> GetByMuscleGroup(MuscleGroup muscleGroup)
    {
        var exercises = await _exerciseService.GetByMuscleGroupAsync(muscleGroup);
        return Ok(exercises);
    }

    /// <summary>
    /// Create a custom exercise
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ExerciseResponse>> Create(
        [FromBody] CreateExerciseRequest request,
        [FromQuery] Guid? userId = null)
    {
        var exercise = await _exerciseService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetById), new { id = exercise.Id }, exercise);
    }

    /// <summary>
    /// Update an exercise
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ExerciseResponse>> Update(Guid id, [FromBody] UpdateExerciseRequest request)
    {
        var exercise = await _exerciseService.UpdateAsync(id, request);
        if (exercise is null) return NotFound();
        return Ok(exercise);
    }

    /// <summary>
    /// Delete a custom exercise (system exercises cannot be deleted)
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var deleted = await _exerciseService.DeleteAsync(id);
        if (!deleted) return BadRequest("Exercise not found or is a system exercise");
        return NoContent();
    }
}

