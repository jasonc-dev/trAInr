using Microsoft.Extensions.Logging;
using trAInr.Application.DTOs;
using trAInr.Application.Interfaces.Repositories;
using trAInr.Application.Interfaces.Services;
using trAInr.Domain.Aggregates;
using trAInr.Domain.Entities;

namespace trAInr.Application.Services;

public class DashboardService(
    IAthleteRepository athleteRepository,
    IAssignedProgramRepository assignedProgramRepository,
    IWorkoutSessionRepository workoutSessionRepository)
    : IDashboardService
{
    public async Task<DashboardResponse> GetDashboardAsync(Guid userId)
    {
        var athlete = await athleteRepository.GetByIdAsync(userId);
        if (athlete is null) throw new InvalidOperationException($"Athlete with ID {userId} not found");

        var activeProgramme = await assignedProgramRepository.GetActiveByAthleteIdAsync(userId);
        ProgrammeSummaryResponse? activeProgrammeSummary = null;
        var currentWeekMetrics = new WeeklyMetrics(0, 0, 0, 0, 0, 0, 0, TimeSpan.Zero);
        var weeklyProgress = new List<WeeklyMetrics>();
        var topExercises = new List<ExerciseMetrics>();
        var overallStats = await GetOverallStatsAsync(userId);

        if (activeProgramme is not null)
        {
            activeProgrammeSummary = MapToProgrammeSummary(activeProgramme);
            weeklyProgress = (await GetWeeklyProgressAsync(activeProgramme.Id)).ToList();

            // Get current week (assuming week 1 for now, could be calculated based on dates)
            currentWeekMetrics = weeklyProgress.FirstOrDefault() ?? currentWeekMetrics;
        }

        topExercises = (await GetExerciseMetricsAsync(userId)).Take(5).ToList();

        return new DashboardResponse(
            activeProgrammeSummary,
            currentWeekMetrics,
            weeklyProgress,
            topExercises,
            overallStats);
    }

    public async Task<IEnumerable<WeeklyMetrics>> GetWeeklyProgressAsync(Guid programmeId)
    {
        var programme = await assignedProgramRepository.GetByIdAsync(programmeId);
        if (programme is null) return Enumerable.Empty<WeeklyMetrics>();

        var metrics = new List<WeeklyMetrics>();

        // Get workout sessions for this program's weeks
        var weekIds = programme.Weeks.Select(w => w.Id).ToList();
        var allSessions = await workoutSessionRepository.GetByAthleteIdAsync(programme.AthleteId);
        var programSessions = allSessions.Where(s => s.ProgrammeWeekId.HasValue && weekIds.Contains(s.ProgrammeWeekId.Value)).ToList();

        foreach (var week in programme.Weeks.OrderBy(w => w.WeekNumber))
        {
            var weekSessions = programSessions
                .Where(s => s.ProgrammeWeekId == week.Id && s.IsCompleted)
                .ToList();

            var workoutsCompleted = weekSessions.Count;
            var workoutsPlanned = week.WorkoutDays.Count(w => !w.IsRestDay);

            var totalVolume = weekSessions
                .SelectMany(s => s.ExerciseInstances)
                .SelectMany(e => e.CompletedSets)
                .Where(s => s.Weight.HasValue && s.Reps.HasValue)
                .Sum(s => s.Weight!.Value * s.Reps!.Value);

            var totalSets = weekSessions
                .SelectMany(s => s.ExerciseInstances)
                .SelectMany(e => e.CompletedSets)
                .Count();

            var totalReps = weekSessions
                .SelectMany(s => s.ExerciseInstances)
                .SelectMany(e => e.CompletedSets)
                .Where(s => s.Reps.HasValue)
                .Sum(s => s.Reps!.Value);

            var totalDuration = TimeSpan.FromSeconds(
                weekSessions
                    .SelectMany(s => s.ExerciseInstances)
                    .SelectMany(e => e.CompletedSets)
                    .Where(s => s.DurationSeconds.HasValue)
                    .Sum(s => s.DurationSeconds!.Value));

            var averageIntensity = weekSessions
                .SelectMany(s => s.ExerciseInstances)
                .SelectMany(e => e.CompletedSets)
                .Where(s => s.RPE.HasValue)
                .Select(s => (decimal)s.RPE!.Value)
                .DefaultIfEmpty(0)
                .Average();

            metrics.Add(new WeeklyMetrics(
                week.WeekNumber,
                totalVolume,
                averageIntensity,
                workoutsCompleted,
                workoutsPlanned,
                totalSets,
                totalReps,
                totalDuration));
        }

        return metrics;
    }

    public async Task<IEnumerable<ExerciseMetrics>> GetExerciseMetricsAsync(Guid userId, int? exerciseId = null)
    {
        var workoutSessions = await workoutSessionRepository.GetByAthleteIdAsync(userId);
        var completedSessions = workoutSessions.Where(s => s.IsCompleted).ToList();

        var exerciseInstances = completedSessions
            .SelectMany(s => s.ExerciseInstances)
            .Where(e => exerciseId == null || e.ExerciseDefinitionId == exerciseId)
            .ToList();

        var exerciseGroups = exerciseInstances
            .GroupBy(e => e.ExerciseDefinitionId)
            .ToList();

        var metrics = new List<ExerciseMetrics>();

        foreach (var group in exerciseGroups)
        {
            var exerciseInstance = group.First();
            var sets = group.SelectMany(e => e.CompletedSets).ToList();

            var totalVolume = sets
                .Where(s => s.Weight.HasValue && s.Reps.HasValue)
                .Sum(s => s.Weight!.Value * s.Reps!.Value);

            var maxWeight = sets
                .Where(s => s.Weight.HasValue)
                .Select(s => s.Weight!.Value)
                .DefaultIfEmpty(0)
                .Max();

            var totalSets = sets.Count;
            var totalReps = sets.Where(s => s.Reps.HasValue).Sum(s => s.Reps!.Value);
            var averageReps = sets.Where(s => s.Reps.HasValue).Select(s => (decimal)s.Reps!.Value).DefaultIfEmpty(0)
                .Average();
            var averageWeight = sets.Where(s => s.Weight.HasValue).Select(s => s.Weight!.Value).DefaultIfEmpty(0)
                .Average();

            // Simplified progress points - in a real implementation, you'd group by date/week
            var progressPoints = sets
                .Where(s => s.CompletedAt.HasValue)
                .GroupBy(s => s.CompletedAt!.Value.Date)
                .Select(g => new ExerciseProgressPoint(
                    g.Key,
                    1, // Week number would need to be calculated
                    g.Where(s => s.Weight.HasValue && s.Reps.HasValue).Sum(s => s.Weight!.Value * s.Reps!.Value),
                    g.Where(s => s.Weight.HasValue).Select(s => s.Weight!.Value).DefaultIfEmpty(0).Max(),
                    g.Where(s => s.Reps.HasValue).Sum(s => s.Reps!.Value),
                    g.Where(s => s.RPE.HasValue).Select(s => (decimal)s.RPE!.Value).DefaultIfEmpty(0)
                        .Average()))
                .ToList();

            metrics.Add(new ExerciseMetrics(
                exerciseInstance.ExerciseDefinitionId,
                exerciseInstance.ExerciseName,
                ExerciseType.WeightTraining, // Would need to get from ExerciseDefinition
                totalVolume,
                maxWeight,
                totalSets,
                totalReps,
                averageReps,
                averageWeight,
                progressPoints));
        }

        return metrics;
    }

    public async Task<OverallStats> GetOverallStatsAsync(Guid userId)
    {
        var workoutSessions = await workoutSessionRepository.GetByAthleteIdAsync(userId);
        var completedSessions = workoutSessions.Where(s => s.IsCompleted).ToList();

        var totalWorkoutsCompleted = completedSessions.Count;
        var totalSetsCompleted = completedSessions
            .SelectMany(s => s.ExerciseInstances)
            .SelectMany(e => e.CompletedSets)
            .Count();

        var totalRepsPerformed = completedSessions
            .SelectMany(s => s.ExerciseInstances)
            .SelectMany(e => e.CompletedSets)
            .Where(s => s.Reps.HasValue)
            .Sum(s => s.Reps!.Value);

        var totalVolumeLifted = completedSessions
            .SelectMany(s => s.ExerciseInstances)
            .SelectMany(e => e.CompletedSets)
            .Where(s => s.Weight.HasValue && s.Reps.HasValue)
            .Sum(s => s.Weight!.Value * s.Reps!.Value);

        var totalTrainingTime = TimeSpan.FromSeconds(
            completedSessions
                .SelectMany(s => s.ExerciseInstances)
                .SelectMany(e => e.CompletedSets)
                .Where(s => s.DurationSeconds.HasValue)
                .Sum(s => s.DurationSeconds!.Value));

        // Simplified streak calculation
        var completedDates = completedSessions
            .Where(s => s.CompletedDate.HasValue)
            .Select(s => s.CompletedDate!.Value.Date)
            .Distinct()
            .OrderByDescending(d => d)
            .ToList();

        var currentStreak = 0;
        var longestStreak = 0;
        var tempStreak = 0;
        var expectedDate = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        foreach (var date in completedDates)
        {
            var dateOnly = DateOnly.FromDateTime(date);
            if (dateOnly == expectedDate || dateOnly == expectedDate.AddDays(-1))
            {
                tempStreak++;
                expectedDate = dateOnly.AddDays(-1);
            }
            else
            {
                if (tempStreak > longestStreak) longestStreak = tempStreak;
                tempStreak = 0;
            }
        }

        currentStreak = tempStreak;
        if (tempStreak > longestStreak) longestStreak = tempStreak;

        return new OverallStats(
            totalWorkoutsCompleted,
            totalSetsCompleted,
            totalRepsPerformed,
            totalVolumeLifted,
            totalTrainingTime,
            currentStreak,
            longestStreak);
    }

    public async Task<IEnumerable<IntensityTrend>> GetIntensityTrendsAsync(Guid programmeId)
    {
        var weeklyProgress = await GetWeeklyProgressAsync(programmeId);
        return weeklyProgress.Select((metrics, index) =>
        {
            var trend = index > 0 ? "increasing" : "stable"; // Simplified
            return new IntensityTrend(
                metrics.WeekNumber,
                metrics.AverageIntensity,
                0, // Average difficulty would need to be calculated
                trend);
        });
    }

    public async Task<IEnumerable<VolumeComparison>> GetVolumeComparisonAsync(Guid programmeId)
    {
        var weeklyProgress = await GetWeeklyProgressAsync(programmeId);
        var progressList = weeklyProgress.ToList();

        return progressList.Select((metrics, index) =>
        {
            var percentageChange = index > 0 && progressList[index - 1].TotalVolume > 0
                ? (metrics.TotalVolume - progressList[index - 1].TotalVolume) / progressList[index - 1].TotalVolume *
                  100
                : 0;

            return new VolumeComparison(
                metrics.WeekNumber,
                metrics.TotalVolume,
                percentageChange);
        });
    }

    private static ProgrammeSummaryResponse MapToProgrammeSummary(AssignedProgram programme)
    {
        var completedWeeks = programme.Weeks.Count(w => w.IsCompleted);
        var progressPercentage = programme.DurationWeeks > 0
            ? (double)completedWeeks / programme.DurationWeeks * 100
            : 0;

        return new ProgrammeSummaryResponse(
            programme.Id,
            programme.Name,
            programme.Description,
            programme.DurationWeeks,
            programme.IsActive,
            false, // IsPreMade - dashboard programmes are assigned programmes
            programme.StartDate,
            completedWeeks,
            progressPercentage);
    }
}