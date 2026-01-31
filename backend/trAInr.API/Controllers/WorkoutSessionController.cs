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

    /// <summary>
    ///     Create a workout day in a programme week
    /// </summary>
    [HttpPost("weeks/{weekId:guid}/days")]
    public async Task<ActionResult<ProgrammeWeekResponse>> CreateWorkoutDay(
        Guid weekId,
        [FromBody] CreateWorkoutDayRequest request)
    {
        var week = await workoutSessionService.CreateWorkoutDayAsync(weekId, request);
        if (week is null) return NotFound("Week not found");
        return Ok(week);
    }

    /// <summary>
    ///     Update a workout day
    /// </summary>
    [HttpPut("days/{id:guid}")]
    public async Task<ActionResult<ProgrammeWeekResponse>> UpdateWorkoutDay(
        Guid id,
        [FromBody] UpdateWorkoutDayRequest request)
    {
        var week = await workoutSessionService.UpdateWorkoutDayAsync(id, request);
        if (week is null) return NotFound();
        return Ok(week);
    }

    /// <summary>
    ///     Delete a workout day
    /// </summary>
    [HttpDelete("days/{id:guid}")]
    public async Task<ActionResult<ProgrammeWeekResponse>> DeleteWorkoutDay(Guid id)
    {
        var week = await workoutSessionService.DeleteWorkoutDayAsync(id);
        if (week is null) return NotFound();
        return Ok(week);
    }

    /// <summary>
    ///     Complete a workout day
    /// </summary>
    [HttpPost("days/{id:guid}/complete")]
    public async Task<ActionResult<ProgrammeWeekResponse>> CompleteWorkout(
        Guid id,
        [FromBody] CompleteWorkoutRequest request)
    {
        var week = await workoutSessionService.CompleteWorkoutAsync(id, request);
        if (week is null) return NotFound();
        return Ok(week);
    }

    #endregion

    #region Workout Exercises

    /// <summary>
    ///     Add an exercise to a workout day
    /// </summary>
    [HttpPost("days/{workoutDayId:guid}/exercises")]
    public async Task<ActionResult<WorkoutDayResponse>> AddExercise(
        Guid workoutDayId,
        [FromBody] AddWorkoutExerciseRequest request)
    {
        var workoutDay = await workoutSessionService.AddExerciseToWorkoutAsync(workoutDayId, request);
        if (workoutDay is null) return NotFound("Workout day or exercise not found");
        return Ok(workoutDay);
    }

    /// <summary>
    ///     Update a workout exercise
    /// </summary>
    [HttpPut("exercises/{id:guid}")]
    public async Task<ActionResult<WorkoutDayResponse>> UpdateExercise(
        Guid id,
        [FromBody] UpdateWorkoutExerciseRequest request)
    {
        var workoutDay = await workoutSessionService.UpdateWorkoutExerciseAsync(id, request);
        if (workoutDay is null) return NotFound();
        return Ok(workoutDay);
    }

    /// <summary>
    ///     Remove an exercise from a workout
    /// </summary>
    [HttpDelete("exercises/{id:guid}")]
    public async Task<ActionResult<WorkoutDayResponse>> RemoveExercise(Guid id)
    {
        var workoutDay = await workoutSessionService.RemoveExerciseFromWorkoutAsync(id);
        if (workoutDay is null) return NotFound();
        return Ok(workoutDay);
    }

    /// <summary>
    ///     Reorder exercises in a workout
    /// </summary>
    [HttpPut("days/{workoutDayId:guid}/exercises/reorder")]
    public async Task<ActionResult<WorkoutDayResponse>> ReorderExercises(
        Guid workoutDayId,
        [FromBody] List<Guid> workoutExerciseIds)
    {
        var workoutDay = await workoutSessionService.ReorderExercisesAsync(workoutDayId, workoutExerciseIds);
        if (workoutDay is null) return BadRequest();
        return Ok(workoutDay);
    }

    #endregion

    #region Exercise Sets

    /// <summary>
    ///     Add a set to a workout exercise
    /// </summary>
    [HttpPost("exercises/{workoutExerciseId:guid}/sets")]
    public async Task<ActionResult<WorkoutExerciseResponse>> AddSet(
        Guid workoutExerciseId,
        [FromBody] CreateExerciseSetRequest request)
    {
        var exercise = await workoutSessionService.AddSetAsync(workoutExerciseId, request);
        if (exercise is null) return NotFound("Workout exercise not found");
        return Ok(exercise);
    }

    /// <summary>
    ///     Update a set
    /// </summary>
    [HttpPut("sets/{id:guid}")]
    public async Task<ActionResult<WorkoutExerciseResponse>> UpdateSet(
        Guid id,
        [FromBody] UpdateExerciseSetRequest request)
    {
        var exercise = await workoutSessionService.UpdateSetAsync(id, request);
        if (exercise is null) return NotFound();
        return Ok(exercise);
    }

    /// <summary>
    ///     Complete a set with tracking data
    /// </summary>
    [HttpPost("sets/{id:guid}/complete")]
    public async Task<ActionResult<WorkoutExerciseResponse>> CompleteSet(
        Guid id,
        [FromBody] CompleteSetRequest request)
    {
        var exercise = await workoutSessionService.CompleteSetAsync(id, request);
        if (exercise is null) return NotFound();
        return Ok(exercise);
    }

    /// <summary>
    ///     Delete a set
    /// </summary>
    [HttpDelete("sets/{id:guid}")]
    public async Task<ActionResult<WorkoutExerciseResponse>> DeleteSet(Guid id)
    {
        var exercise = await workoutSessionService.DeleteSetAsync(id);
        if (exercise is null) return NotFound();
        return Ok(exercise);
    }

    #endregion

    #region Superset and Drop Set Operations

    /// <summary>
    ///     Group multiple exercises into a superset
    /// </summary>
    [HttpPut("days/{workoutDayId:guid}/exercises/superset")]
    public async Task<ActionResult<WorkoutDayResponse>> GroupExercisesInSuperset(
        Guid workoutDayId,
        [FromBody] GroupSupersetRequest request)
    {
        var workoutDay = await workoutSessionService.GroupExercisesInSupersetAsync(workoutDayId, request);
        if (workoutDay is null) return BadRequest("Failed to group exercises. Ensure at least 2 exercises are provided.");
        return Ok(workoutDay);
    }

    /// <summary>
    ///     Ungroup exercises from a superset
    /// </summary>
    [HttpDelete("exercises/superset/{supersetGroupId:guid}")]
    public async Task<ActionResult<WorkoutDayResponse>> UngroupExercisesFromSuperset(Guid supersetGroupId)
    {
        var workoutDay = await workoutSessionService.UngroupExercisesFromSupersetAsync(supersetGroupId);
        if (workoutDay is null) return NotFound("Superset group not found");
        return Ok(workoutDay);
    }

    /// <summary>
    ///     Create a drop set sequence for an exercise
    /// </summary>
    [HttpPost("exercises/{workoutExerciseId:guid}/dropsets")]
    public async Task<ActionResult<WorkoutExerciseResponse>> CreateDropSetSequence(
        Guid workoutExerciseId,
        [FromBody] CreateDropSetRequest request)
    {
        var exercise = await workoutSessionService.CreateDropSetSequenceAsync(workoutExerciseId, request);
        if (exercise is null) return NotFound("Workout exercise not found");
        return Ok(exercise);
    }

    #endregion
}