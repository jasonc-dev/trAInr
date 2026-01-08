using Microsoft.AspNetCore.Mvc;
using trAInr.Application.DTOs;
using trAInr.Application.Interfaces.Services;

namespace trAInr.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkoutSessionController(IWorkoutSessionService workoutSessionService) : ControllerBase
{
    #region Workout Days

    /// <summary>
    ///     Get a workout day by ID
    /// </summary>
    [HttpGet("days/{id:guid}")]
    public async Task<ActionResult<WorkoutDayResponse>> GetWorkoutDay(Guid id)
    {
        var workoutDay = await workoutSessionService.GetWorkoutDayAsync(id);
        if (workoutDay is null) return NotFound();
        return Ok(workoutDay);
    }

    [HttpGet("weeks/{weekId:guid}/days")]
    public async Task<ActionResult<IEnumerable<WorkoutDayResponse>>> GetWorkoutDays(Guid weekId)
    {
        var workoutDays = await workoutSessionService.GetWorkoutDaysAsync(weekId);
        if (workoutDays is null) return NotFound();
        return Ok(workoutDays);
    }

    /// <summary>
    ///     Create a workout day in a programme week
    /// </summary>
    [HttpPost("weeks/{weekId:guid}/days")]
    public async Task<ActionResult<WorkoutDayResponse>> CreateWorkoutDay(
        Guid weekId,
        [FromBody] CreateWorkoutDayRequest request)
    {
        var workoutDay = await workoutSessionService.CreateWorkoutDayAsync(weekId, request);
        if (workoutDay is null) return NotFound("Week not found");
        return CreatedAtAction(nameof(GetWorkoutDay), new { id = workoutDay.Id }, workoutDay);
    }

    /// <summary>
    ///     Update a workout day
    /// </summary>
    [HttpPut("days/{id:guid}")]
    public async Task<ActionResult<WorkoutDayResponse>> UpdateWorkoutDay(
        Guid id,
        [FromBody] UpdateWorkoutDayRequest request)
    {
        var workoutDay = await workoutSessionService.UpdateWorkoutDayAsync(id, request);
        if (workoutDay is null) return NotFound();
        return Ok(workoutDay);
    }

    /// <summary>
    ///     Delete a workout day
    /// </summary>
    [HttpDelete("days/{id:guid}")]
    public async Task<ActionResult> DeleteWorkoutDay(Guid id)
    {
        var deleted = await workoutSessionService.DeleteWorkoutDayAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    /// <summary>
    ///     Complete a workout day
    /// </summary>
    [HttpPost("days/{id:guid}/complete")]
    public async Task<ActionResult<WorkoutDayResponse>> CompleteWorkout(
        Guid id,
        [FromBody] CompleteWorkoutRequest request)
    {
        var workoutDay = await workoutSessionService.CompleteWorkoutAsync(id, request);
        if (workoutDay is null) return NotFound();
        return Ok(workoutDay);
    }

    #endregion

    #region Workout Exercises

    /// <summary>
    ///     Add an exercise to a workout day
    /// </summary>
    [HttpPost("days/{workoutDayId:guid}/exercises")]
    public async Task<ActionResult<WorkoutExerciseResponse>> AddExercise(
        Guid workoutDayId,
        [FromBody] AddWorkoutExerciseRequest request)
    {
        var exercise = await workoutSessionService.AddExerciseToWorkoutAsync(workoutDayId, request);
        if (exercise is null) return NotFound("Workout day or exercise not found");
        return Ok(exercise);
    }

    /// <summary>
    ///     Update a workout exercise
    /// </summary>
    [HttpPut("exercises/{id:guid}")]
    public async Task<ActionResult<WorkoutExerciseResponse>> UpdateExercise(
        Guid id,
        [FromBody] UpdateWorkoutExerciseRequest request)
    {
        var exercise = await workoutSessionService.UpdateWorkoutExerciseAsync(id, request);
        if (exercise is null) return NotFound();
        return Ok(exercise);
    }

    /// <summary>
    ///     Remove an exercise from a workout
    /// </summary>
    [HttpDelete("exercises/{id:guid}")]
    public async Task<ActionResult> RemoveExercise(Guid id)
    {
        var removed = await workoutSessionService.RemoveExerciseFromWorkoutAsync(id);
        if (!removed) return NotFound();
        return NoContent();
    }

    /// <summary>
    ///     Reorder exercises in a workout
    /// </summary>
    [HttpPut("days/{workoutDayId:guid}/exercises/reorder")]
    public async Task<ActionResult> ReorderExercises(
        Guid workoutDayId,
        [FromBody] List<Guid> exerciseIds)
    {
        var reordered = await workoutSessionService.ReorderExercisesAsync(workoutDayId, exerciseIds);
        if (!reordered) return BadRequest();
        return NoContent();
    }

    #endregion

    #region Exercise Sets

    /// <summary>
    ///     Add a set to a workout exercise
    /// </summary>
    [HttpPost("exercises/{workoutExerciseId:guid}/sets")]
    public async Task<ActionResult<ExerciseSetResponse>> AddSet(
        Guid workoutExerciseId,
        [FromBody] CreateExerciseSetRequest request)
    {
        var set = await workoutSessionService.AddSetAsync(workoutExerciseId, request);
        if (set is null) return NotFound("Workout exercise not found");
        return Ok(set);
    }

    /// <summary>
    ///     Update a set
    /// </summary>
    [HttpPut("sets/{id:guid}")]
    public async Task<ActionResult<ExerciseSetResponse>> UpdateSet(
        Guid id,
        [FromBody] UpdateExerciseSetRequest request)
    {
        var set = await workoutSessionService.UpdateSetAsync(id, request);
        if (set is null) return NotFound();
        return Ok(set);
    }

    /// <summary>
    ///     Complete a set with tracking data
    /// </summary>
    [HttpPost("sets/{id:guid}/complete")]
    public async Task<ActionResult<ExerciseSetResponse>> CompleteSet(
        Guid id,
        [FromBody] CompleteSetRequest request)
    {
        var set = await workoutSessionService.CompleteSetAsync(id, request);
        if (set is null) return NotFound();
        return Ok(set);
    }

    /// <summary>
    ///     Delete a set
    /// </summary>
    [HttpDelete("sets/{id:guid}")]
    public async Task<ActionResult> DeleteSet(Guid id)
    {
        var deleted = await workoutSessionService.DeleteSetAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    #endregion

    #region Superset and Drop Set Operations

    /// <summary>
    ///     Group multiple exercises into a superset
    /// </summary>
    [HttpPut("days/{workoutDayId:guid}/exercises/superset")]
    public async Task<ActionResult<IEnumerable<WorkoutExerciseResponse>>> GroupExercisesInSuperset(
        Guid workoutDayId,
        [FromBody] GroupSupersetRequest request)
    {
        var exercises = await workoutSessionService.GroupExercisesInSupersetAsync(workoutDayId, request);
        if (exercises is null) return BadRequest("Failed to group exercises. Ensure at least 2 exercises are provided.");
        return Ok(exercises);
    }

    /// <summary>
    ///     Ungroup exercises from a superset
    /// </summary>
    [HttpDelete("exercises/superset/{supersetGroupId:guid}")]
    public async Task<ActionResult> UngroupExercisesFromSuperset(Guid supersetGroupId)
    {
        var ungrouped = await workoutSessionService.UngroupExercisesFromSupersetAsync(supersetGroupId);
        if (!ungrouped) return NotFound("Superset group not found");
        return NoContent();
    }

    /// <summary>
    ///     Create a drop set sequence for an exercise
    /// </summary>
    [HttpPost("exercises/{workoutExerciseId:guid}/dropsets")]
    public async Task<ActionResult<IEnumerable<ExerciseSetResponse>>> CreateDropSetSequence(
        Guid workoutExerciseId,
        [FromBody] CreateDropSetRequest request)
    {
        var sets = await workoutSessionService.CreateDropSetSequenceAsync(workoutExerciseId, request);
        if (sets is null) return NotFound("Workout exercise not found");
        return Ok(sets);
    }

    #endregion
}