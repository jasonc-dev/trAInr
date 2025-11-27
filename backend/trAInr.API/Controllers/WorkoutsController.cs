using Microsoft.AspNetCore.Mvc;
using trAInr.API.Models.DTOs;
using trAInr.API.Services;

namespace trAInr.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkoutsController : ControllerBase
{
    private readonly IWorkoutService _workoutService;

    public WorkoutsController(IWorkoutService workoutService)
    {
        _workoutService = workoutService;
    }

    #region Workout Days

    /// <summary>
    /// Get a workout day by ID
    /// </summary>
    [HttpGet("days/{id:guid}")]
    public async Task<ActionResult<WorkoutDayResponse>> GetWorkoutDay(Guid id)
    {
        var workoutDay = await _workoutService.GetWorkoutDayAsync(id);
        if (workoutDay is null) return NotFound();
        return Ok(workoutDay);
    }

    /// <summary>
    /// Create a workout day in a week
    /// </summary>
    [HttpPost("weeks/{weekId:guid}/days")]
    public async Task<ActionResult<WorkoutDayResponse>> CreateWorkoutDay(
        Guid weekId,
        [FromBody] CreateWorkoutDayRequest request)
    {
        var workoutDay = await _workoutService.CreateWorkoutDayAsync(weekId, request);
        if (workoutDay is null) return NotFound("Week not found");
        return CreatedAtAction(nameof(GetWorkoutDay), new { id = workoutDay.Id }, workoutDay);
    }

    /// <summary>
    /// Update a workout day
    /// </summary>
    [HttpPut("days/{id:guid}")]
    public async Task<ActionResult<WorkoutDayResponse>> UpdateWorkoutDay(
        Guid id,
        [FromBody] UpdateWorkoutDayRequest request)
    {
        var workoutDay = await _workoutService.UpdateWorkoutDayAsync(id, request);
        if (workoutDay is null) return NotFound();
        return Ok(workoutDay);
    }

    /// <summary>
    /// Delete a workout day
    /// </summary>
    [HttpDelete("days/{id:guid}")]
    public async Task<ActionResult> DeleteWorkoutDay(Guid id)
    {
        var deleted = await _workoutService.DeleteWorkoutDayAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Complete a workout day
    /// </summary>
    [HttpPost("days/{id:guid}/complete")]
    public async Task<ActionResult<WorkoutDayResponse>> CompleteWorkout(
        Guid id,
        [FromBody] CompleteWorkoutRequest request)
    {
        var workoutDay = await _workoutService.CompleteWorkoutAsync(id, request);
        if (workoutDay is null) return NotFound();
        return Ok(workoutDay);
    }

    #endregion

    #region Workout Exercises

    /// <summary>
    /// Add an exercise to a workout day
    /// </summary>
    [HttpPost("days/{workoutDayId:guid}/exercises")]
    public async Task<ActionResult<WorkoutExerciseResponse>> AddExercise(
        Guid workoutDayId,
        [FromBody] AddWorkoutExerciseRequest request)
    {
        var exercise = await _workoutService.AddExerciseToWorkoutAsync(workoutDayId, request);
        if (exercise is null) return NotFound("Workout day or exercise not found");
        return Ok(exercise);
    }

    /// <summary>
    /// Update a workout exercise
    /// </summary>
    [HttpPut("exercises/{id:guid}")]
    public async Task<ActionResult<WorkoutExerciseResponse>> UpdateExercise(
        Guid id,
        [FromBody] UpdateWorkoutExerciseRequest request)
    {
        var exercise = await _workoutService.UpdateWorkoutExerciseAsync(id, request);
        if (exercise is null) return NotFound();
        return Ok(exercise);
    }

    /// <summary>
    /// Remove an exercise from a workout
    /// </summary>
    [HttpDelete("exercises/{id:guid}")]
    public async Task<ActionResult> RemoveExercise(Guid id)
    {
        var removed = await _workoutService.RemoveExerciseFromWorkoutAsync(id);
        if (!removed) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Reorder exercises in a workout
    /// </summary>
    [HttpPut("days/{workoutDayId:guid}/exercises/reorder")]
    public async Task<ActionResult> ReorderExercises(
        Guid workoutDayId,
        [FromBody] List<Guid> exerciseIds)
    {
        var reordered = await _workoutService.ReorderExercisesAsync(workoutDayId, exerciseIds);
        if (!reordered) return BadRequest();
        return NoContent();
    }

    #endregion

    #region Exercise Sets

    /// <summary>
    /// Add a set to a workout exercise
    /// </summary>
    [HttpPost("exercises/{workoutExerciseId:guid}/sets")]
    public async Task<ActionResult<ExerciseSetResponse>> AddSet(
        Guid workoutExerciseId,
        [FromBody] CreateExerciseSetRequest request)
    {
        var set = await _workoutService.AddSetAsync(workoutExerciseId, request);
        if (set is null) return NotFound("Workout exercise not found");
        return Ok(set);
    }

    /// <summary>
    /// Update a set
    /// </summary>
    [HttpPut("sets/{id:guid}")]
    public async Task<ActionResult<ExerciseSetResponse>> UpdateSet(
        Guid id,
        [FromBody] UpdateExerciseSetRequest request)
    {
        var set = await _workoutService.UpdateSetAsync(id, request);
        if (set is null) return NotFound();
        return Ok(set);
    }

    /// <summary>
    /// Complete a set with tracking data
    /// </summary>
    [HttpPost("sets/{id:guid}/complete")]
    public async Task<ActionResult<ExerciseSetResponse>> CompleteSet(
        Guid id,
        [FromBody] CompleteSetRequest request)
    {
        var set = await _workoutService.CompleteSetAsync(id, request);
        if (set is null) return NotFound();
        return Ok(set);
    }

    /// <summary>
    /// Delete a set
    /// </summary>
    [HttpDelete("sets/{id:guid}")]
    public async Task<ActionResult> DeleteSet(Guid id)
    {
        var deleted = await _workoutService.DeleteSetAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    #endregion
}

