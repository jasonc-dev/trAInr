using Microsoft.EntityFrameworkCore;
using trAInr.Application.DTOs.AI;
using trAInr.Application.Interfaces.Services.AI;
using trAInr.Domain.Entities;
using trAInr.Domain.Enums;
using trAInr.Infrastructure.Data;

namespace trAInr.Infrastructure.Services;

public class ExerciseRetrievalService : IExerciseRetrievalService
{
    private readonly TrainrDbContext _context;

    public ExerciseRetrievalService(TrainrDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ExercisePromptDto>> GetCandidateExercisesAsync(
        ExerciseRetrievalRequest request,
        CancellationToken cancellationToken = default)
    {
        // Start with active exercises
        var query = _context.ExerciseDefinitions
            .Include(e => e.ExerciseEquipments).ThenInclude(ee => ee.Equipment)
            .Include(e => e.ExerciseMuscles).ThenInclude(em => em.Muscle)
            .Include(e => e.ExerciseTags).ThenInclude(et => et.Tag)
            .Where(e => e.IsSystemExercise) // Only system exercises for now
            .AsQueryable();

        // Filter by difficulty
        query = query.Where(e => e.LevelOfDifficulty <= request.MaxDifficulty);

        // Exclude flexibility for program generation
        query = query.Where(e => e.Type != ExerciseType.Flexibility);

        // Fetch exercises into memory for complex filtering
        var exercises = await query.ToListAsync(cancellationToken);

        // Apply equipment filter: exercises that can be done with available equipment
        if (request.AvailableEquipment.Length > 0)
        {
            var availableEquipmentSet = request.AvailableEquipment.ToHashSet(StringComparer.OrdinalIgnoreCase);

            exercises = exercises.Where(e =>
            {
                var requiredEquipment = e.ExerciseEquipments.Select(ee => ee.Equipment.Name).ToList();

                // If no equipment specified or bodyweight, it's always available
                if (!requiredEquipment.Any() || e.IsBodyweight)
                    return true;

                // Check if all required equipment is available
                return requiredEquipment.All(eq => availableEquipmentSet.Contains(eq));
            }).ToList();
        }

        // Apply tag filtering
        if (request.ExcludedTags?.Length > 0)
        {
            var excludedTagSet = request.ExcludedTags.ToHashSet(StringComparer.OrdinalIgnoreCase);
            exercises = exercises.Where(e =>
            {
                var exerciseTags = e.ExerciseTags.Select(et => $"{et.Tag.Namespace}:{et.Tag.Name}").ToList();
                return !exerciseTags.Any(tag => excludedTagSet.Contains(tag));
            }).ToList();
        }

        if (request.RequiredTags?.Length > 0)
        {
            var requiredTagSet = request.RequiredTags.ToHashSet(StringComparer.OrdinalIgnoreCase);
            exercises = exercises.Where(e =>
            {
                var exerciseTags = e.ExerciseTags.Select(et => $"{et.Tag.Namespace}:{et.Tag.Name}").ToList();
                return requiredTagSet.All(tag => exerciseTags.Contains(tag));
            }).ToList();
        }

        // Apply diversity: limit per movement pattern to avoid returning 30 curls
        var diverseExercises = exercises
            .GroupBy(e => e.MovementPattern)
            .SelectMany(g => g.Take(request.PerMovementPatternLimit))
            .Take(request.Limit)
            .ToList();

        // Project to ExercisePromptDto
        return diverseExercises.Select(e => new ExercisePromptDto(
            Id: e.Id,
            Name: e.Name,
            MovementPattern: e.MovementPattern.ToString(),
            ExerciseType: e.Type.ToString(),
            Equipment: e.ExerciseEquipments.Select(ee => ee.Equipment.Name).ToArray(),
            PrimaryMuscles: e.ExerciseMuscles
                .Where(em => em.Role == MuscleRole.Primary)
                .Select(em => em.Muscle.Name)
                .ToArray(),
            SecondaryMuscles: e.ExerciseMuscles
                .Where(em => em.Role == MuscleRole.Secondary)
                .Select(em => em.Muscle.Name)
                .ToArray(),
            Difficulty: e.LevelOfDifficulty.ToString(),
            SpinalLoad: e.SpinalLoad.ToString(),
            SetupComplexity: e.SetupComplexity.ToString(),
            TrackingMode: e.TrackingMode.ToString(),
            DefaultRepRange: e.DefaultRepMin.HasValue && e.DefaultRepMax.HasValue
                ? $"{e.DefaultRepMin}-{e.DefaultRepMax}"
                : null,
            Tags: e.ExerciseTags.Select(et => $"{et.Tag.Namespace}:{et.Tag.Name}").ToArray(),
            ShortCue: e.ShortCue
        )).ToList();
    }
}
