using Microsoft.AspNetCore.Mvc;
using trAInr.Application.DTOs;
using trAInr.Application.Interfaces.Services;
using trAInr.Domain.Entities;

namespace trAInr.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExerciseDefinitionController(IExerciseDefinitionService exerciseDefinitionService) : ControllerBase
{
    /// <summary>
    ///     Get all exercise definitions
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExerciseResponse>>> GetAll()
    {
        var exercises = await exerciseDefinitionService.GetAllAsync();
        return Ok(exercises);
    }

    /// <summary>
    ///     Search exercise definitions with optional filters
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ExerciseSummaryResponse>>> Search(
        [FromQuery] string? query = null,
        [FromQuery] ExerciseType? type = null,
        [FromQuery] MuscleGroup? muscleGroup = null)
    {
        var exercises = await exerciseDefinitionService.SearchAsync(query, type, muscleGroup);
        return Ok(exercises);
    }

    /// <summary>
    ///     Get exercise definition by ID
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ExerciseResponse>> GetById(int id)
    {
        var exercise = await exerciseDefinitionService.GetByIdAsync(id);
        if (exercise is null) return NotFound();
        return Ok(exercise);
    }

    /// <summary>
    ///     Get exercise definitions by type
    /// </summary>
    [HttpGet("type/{type}")]
    public async Task<ActionResult<IEnumerable<ExerciseSummaryResponse>>> GetByType(ExerciseType type)
    {
        var exercises = await exerciseDefinitionService.GetByTypeAsync(type);
        return Ok(exercises);
    }

    /// <summary>
    ///     Get exercise definitions by muscle group
    /// </summary>
    [HttpGet("muscle-group/{muscleGroup}")]
    public async Task<ActionResult<IEnumerable<ExerciseSummaryResponse>>> GetByMuscleGroup(MuscleGroup muscleGroup)
    {
        var exercises = await exerciseDefinitionService.GetByMuscleGroupAsync(muscleGroup);
        return Ok(exercises);
    }

}