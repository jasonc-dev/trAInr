using System.Text.Json;
using Microsoft.Extensions.Logging;
using trAInr.Application.DTOs;
using trAInr.Application.Interfaces;
using trAInr.Application.Interfaces.Repositories;
using trAInr.Application.Interfaces.Services;
using trAInr.Domain.Entities;

#pragma warning disable CA1822 // Mark members as static - these methods will be extended

namespace trAInr.Application.Services;

/// <summary>
///     Service for handling offline sync operations
/// </summary>
public class SyncService(
    IWorkoutSessionRepository workoutSessionRepository,
    IIdempotencyRepository idempotencyRepository,
    IUnitOfWork unitOfWork,
    ILogger<SyncService> logger)
    : ISyncService
{
    public async Task<SyncPullResponse> PullChangesAsync(Guid athleteId, SyncPullRequest request)
    {
        var since = request.Since ?? DateTime.MinValue;
        var changes = new List<SyncChange>();

        // Get modified workout days
        var modifiedWorkoutDays = await workoutSessionRepository.GetModifiedSinceAsync(athleteId, since);

        foreach (var day in modifiedWorkoutDays)
        {
            changes.Add(new SyncChange
            {
                EntityType = "workout_days",
                EntityId = day.Id.ToString(),
                Operation = "update",
                Data = new
                {
                    day.Id,
                    day.ProgrammeWeekId,
                    day.Name,
                    day.Description,
                    day.ScheduledDate,
                    day.CompletedDate,
                    day.IsCompleted,
                    day.IsRestDay,
                    day.CreatedAt
                },
                ClientTimestamp = day.CreatedAt
            });

            // Include exercises for this day
            foreach (var exercise in day.Exercises)
            {
                changes.Add(new SyncChange
                {
                    EntityType = "workout_exercises",
                    EntityId = exercise.Id.ToString(),
                    Operation = "update",
                    Data = new
                    {
                        exercise.Id,
                        exercise.WorkoutDayId,
                        exercise.ExerciseDefinitionId,
                        exercise.OrderIndex,
                        exercise.TargetSets,
                        exercise.TargetReps,
                        exercise.TargetWeight,
                        exercise.RestSeconds,
                        exercise.Notes,
                        exercise.CreatedAt
                    },
                    ClientTimestamp = exercise.CreatedAt
                });

                // Include sets for this exercise
                foreach (var set in exercise.Sets)
                {
                    changes.Add(new SyncChange
                    {
                        EntityType = "exercise_sets",
                        EntityId = set.Id.ToString(),
                        Operation = "update",
                        Data = new
                        {
                            set.Id,
                            set.WorkoutExerciseId,
                            set.SetNumber,
                            set.Reps,
                            set.Weight,
                            set.IsCompleted,
                            set.CompletedAt,
                            set.Notes,
                            set.CreatedAt
                        },
                        ClientTimestamp = set.CreatedAt
                    });
                }
            }
        }

        return new SyncPullResponse
        {
            ServerTimestamp = DateTime.UtcNow,
            Changes = changes,
            HasMore = false
        };
    }

    public async Task<SyncPushResponse> PushChangesAsync(Guid athleteId, SyncPushRequest request)
    {
        var results = new List<SyncChangeResult>();

        foreach (var change in request.Changes)
        {
            try
            {
                // Check if this idempotency key was already processed
                var existingRecord = await idempotencyRepository.GetByKeyAsync(change.IdempotencyKey, athleteId);
                if (existingRecord != null)
                {
                    results.Add(new SyncChangeResult
                    {
                        IdempotencyKey = change.IdempotencyKey,
                        EntityId = change.EntityId,
                        Status = SyncResultStatus.AlreadyProcessed
                    });
                    continue;
                }

                var result = await ProcessChangeAsync(athleteId, change);
                results.Add(result);

                // Record the idempotency key
                var idempotencyRecord = new IdempotencyRecord(
                    Guid.NewGuid(),
                    change.IdempotencyKey,
                    athleteId,
                    change.EntityType,
                    change.EntityId,
                    change.Operation,
                    result.ServerData != null ? JsonSerializer.Serialize(result.ServerData) : null);

                await idempotencyRepository.AddAsync(idempotencyRecord);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process sync change {IdempotencyKey}", change.IdempotencyKey);
                results.Add(new SyncChangeResult
                {
                    IdempotencyKey = change.IdempotencyKey,
                    EntityId = change.EntityId,
                    Status = SyncResultStatus.ValidationError,
                    ErrorMessage = ex.Message
                });
            }
        }

        await unitOfWork.SaveChangesAsync();

        return new SyncPushResponse
        {
            ServerTimestamp = DateTime.UtcNow,
            Results = results
        };
    }

    public async Task<object> GetSyncStatusAsync(Guid athleteId)
    {
        var lastSync = await idempotencyRepository.GetLastSyncTimeAsync(athleteId);

        return new
        {
            ServerTimestamp = DateTime.UtcNow,
            LastSyncTimestamp = lastSync,
            PendingChangesCount = 0
        };
    }

    private async Task<SyncChangeResult> ProcessChangeAsync(Guid athleteId, SyncChange change)
    {
        return change.EntityType switch
        {
            "exercise_sets" => await ProcessExerciseSetChangeAsync(athleteId, change),
            "workout_exercises" => await ProcessWorkoutExerciseChangeAsync(athleteId, change),
            "workout_days" => await ProcessWorkoutDayChangeAsync(athleteId, change),
            _ => new SyncChangeResult
            {
                IdempotencyKey = change.IdempotencyKey,
                EntityId = change.EntityId,
                Status = SyncResultStatus.ValidationError,
                ErrorMessage = $"Unknown entity type: {change.EntityType}"
            }
        };
    }

    private async Task<SyncChangeResult> ProcessExerciseSetChangeAsync(Guid athleteId, SyncChange change)
    {
        if (change.Data == null)
        {
            return new SyncChangeResult
            {
                IdempotencyKey = change.IdempotencyKey,
                EntityId = change.EntityId,
                Status = SyncResultStatus.ValidationError,
                ErrorMessage = "No data provided"
            };
        }

        var jsonElement = (JsonElement)change.Data;

        switch (change.Operation)
        {
            case "update":
                var setId = Guid.Parse(change.EntityId);
                var set = await workoutSessionRepository.GetExerciseSetByIdAsync(setId);

                if (set == null)
                {
                    return new SyncChangeResult
                    {
                        IdempotencyKey = change.IdempotencyKey,
                        EntityId = change.EntityId,
                        Status = SyncResultStatus.NotFound
                    };
                }

                // Update set properties
                if (jsonElement.TryGetProperty("reps", out var reps))
                    set.Reps = reps.GetInt32();
                if (jsonElement.TryGetProperty("weight", out var weight))
                    set.Weight = weight.GetDecimal();
                if (jsonElement.TryGetProperty("isCompleted", out var isCompleted))
                    set.IsCompleted = isCompleted.GetBoolean();
                if (jsonElement.TryGetProperty("completedAt", out var completedAt) && completedAt.ValueKind != JsonValueKind.Null)
                    set.CompletedAt = completedAt.GetDateTime();
                if (jsonElement.TryGetProperty("notes", out var notes))
                    set.Notes = notes.GetString();

                await workoutSessionRepository.UpdateExerciseSetAsync(set);

                return new SyncChangeResult
                {
                    IdempotencyKey = change.IdempotencyKey,
                    EntityId = change.EntityId,
                    Status = SyncResultStatus.Success,
                    NewVersion = 1,
                    ServerData = set
                };

            default:
                return new SyncChangeResult
                {
                    IdempotencyKey = change.IdempotencyKey,
                    EntityId = change.EntityId,
                    Status = SyncResultStatus.ValidationError,
                    ErrorMessage = $"Unsupported operation: {change.Operation}"
                };
        }
    }

    private Task<SyncChangeResult> ProcessWorkoutExerciseChangeAsync(Guid athleteId, SyncChange change)
    {
        // Simplified implementation - extend as needed
        return Task.FromResult(new SyncChangeResult
        {
            IdempotencyKey = change.IdempotencyKey,
            EntityId = change.EntityId,
            Status = SyncResultStatus.Success
        });
    }

    private Task<SyncChangeResult> ProcessWorkoutDayChangeAsync(Guid athleteId, SyncChange change)
    {
        // Simplified implementation - extend as needed
        return Task.FromResult(new SyncChangeResult
        {
            IdempotencyKey = change.IdempotencyKey,
            EntityId = change.EntityId,
            Status = SyncResultStatus.Success
        });
    }
}
