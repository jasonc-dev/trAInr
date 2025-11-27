using Microsoft.EntityFrameworkCore;
using trAInr.API.Data;
using trAInr.API.Models.Domain;
using trAInr.API.Models.DTOs;

namespace trAInr.API.Services;

public class DashboardService : IDashboardService
{
  private readonly TrainrDbContext _context;
  private readonly IProgrammeService _programmeService;

  public DashboardService(TrainrDbContext context, IProgrammeService programmeService)
  {
    _context = context;
    _programmeService = programmeService;
  }

  public async Task<DashboardResponse> GetDashboardAsync(Guid userId)
  {
    var activeProgramme = await _programmeService.GetActiveByUserIdAsync(userId);
    var weeklyProgress = activeProgramme is not null
        ? await GetWeeklyProgressAsync(activeProgramme.Id)
        : Enumerable.Empty<WeeklyMetrics>();

    var currentWeekMetrics = weeklyProgress.LastOrDefault() ?? new WeeklyMetrics(
        0, 0, 0, 0, 0, 0, 0, TimeSpan.Zero);

    var topExercises = await GetExerciseMetricsAsync(userId);
    var overallStats = await GetOverallStatsAsync(userId);

    return new DashboardResponse(
        activeProgramme,
        currentWeekMetrics,
        weeklyProgress,
        topExercises.Take(5),
        overallStats);
  }

  public async Task<IEnumerable<WeeklyMetrics>> GetWeeklyProgressAsync(Guid programmeId)
  {
    var programme = await _context.Programmes
        .Include(p => p.Weeks)
            .ThenInclude(w => w.WorkoutDays)
                .ThenInclude(d => d.Exercises)
                    .ThenInclude(e => e.Sets)
        .FirstOrDefaultAsync(p => p.Id == programmeId);

    if (programme is null) return Enumerable.Empty<WeeklyMetrics>();

    var metrics = new List<WeeklyMetrics>();

    foreach (var week in programme.Weeks.OrderBy(w => w.WeekNumber))
    {
      var completedSets = week.WorkoutDays
          .SelectMany(d => d.Exercises)
          .SelectMany(e => e.Sets)
          .Where(s => s.IsCompleted)
          .ToList();

      var totalVolume = completedSets
          .Where(s => s.Weight.HasValue && s.Reps.HasValue)
          .Sum(s => (s.Weight ?? 0) * (s.Reps ?? 0));

      var avgIntensity = completedSets
          .Where(s => s.Intensity.HasValue)
          .Select(s => (int)s.Intensity!.Value)
          .DefaultIfEmpty(0)
          .Average();

      var totalDuration = TimeSpan.FromSeconds(completedSets
          .Where(s => s.DurationSeconds.HasValue)
          .Sum(s => s.DurationSeconds ?? 0));

      var workoutsCompleted = week.WorkoutDays.Count(d => d.IsCompleted && !d.IsRestDay);
      var workoutsPlanned = week.WorkoutDays.Count(d => !d.IsRestDay);
      var totalReps = completedSets.Sum(s => s.Reps ?? 0);

      metrics.Add(new WeeklyMetrics(
          week.WeekNumber,
          totalVolume,
          (decimal)avgIntensity,
          workoutsCompleted,
          workoutsPlanned,
          completedSets.Count,
          totalReps,
          totalDuration));
    }

    return metrics;
  }

  public async Task<IEnumerable<ExerciseMetrics>> GetExerciseMetricsAsync(Guid userId, Guid? exerciseId = null)
  {
    var query = _context.ExerciseSets
        .Include(s => s.WorkoutExercise)
            .ThenInclude(e => e.Exercise)
        .Include(s => s.WorkoutExercise)
            .ThenInclude(e => e.WorkoutDay)
                .ThenInclude(d => d.ProgrammeWeek)
                    .ThenInclude(w => w.Programme)
        .Where(s => s.IsCompleted &&
                    s.WorkoutExercise.WorkoutDay.ProgrammeWeek.Programme.UserId == userId);

    if (exerciseId.HasValue)
    {
      query = query.Where(s => s.WorkoutExercise.ExerciseId == exerciseId.Value);
    }

    var sets = await query.ToListAsync();

    var grouped = sets
        .GroupBy(s => new { s.WorkoutExercise.ExerciseId, s.WorkoutExercise.Exercise.Name, s.WorkoutExercise.Exercise.Type })
        .Select(g =>
        {
          var allSets = g.ToList();
          var totalVolume = allSets
                  .Where(s => s.Weight.HasValue && s.Reps.HasValue)
                  .Sum(s => (s.Weight ?? 0) * (s.Reps ?? 0));
          var maxWeight = allSets
                  .Where(s => s.Weight.HasValue)
                  .Max(s => s.Weight ?? 0);
          var totalReps = allSets.Sum(s => s.Reps ?? 0);
          var avgReps = allSets.Count > 0
                  ? (decimal)allSets.Average(s => s.Reps ?? 0)
                  : 0;
          var avgWeight = allSets
                  .Where(s => s.Weight.HasValue)
                  .DefaultIfEmpty()
                  .Average(s => s?.Weight ?? 0);

          var progressPoints = allSets
                  .Where(s => s.CompletedAt.HasValue)
                  .GroupBy(s => new
                {
                  s.CompletedAt!.Value.Date,
                  s.WorkoutExercise.WorkoutDay.ProgrammeWeek.WeekNumber
                })
                  .Select(pg => new ExerciseProgressPoint(
                      pg.Key.Date,
                      pg.Key.WeekNumber,
                      pg.Where(x => x.Weight.HasValue && x.Reps.HasValue)
                          .Sum(x => (x.Weight ?? 0) * (x.Reps ?? 0)),
                      pg.Max(x => x.Weight ?? 0),
                      pg.Sum(x => x.Reps ?? 0),
                      pg.Where(x => x.Intensity.HasValue)
                          .Select(x => (int)x.Intensity!.Value)
                          .DefaultIfEmpty(0)
                          .Average() is var avg ? (decimal)avg : 0))
                  .OrderBy(p => p.Date)
                  .ToList();

          return new ExerciseMetrics(
                  g.Key.ExerciseId,
                  g.Key.Name,
                  g.Key.Type,
                  totalVolume,
                  maxWeight,
                  allSets.Count,
                  totalReps,
                  avgReps,
                  avgWeight,
                  progressPoints);
        })
        .OrderByDescending(m => m.TotalVolume)
        .ToList();

    return grouped;
  }

  public async Task<OverallStats> GetOverallStatsAsync(Guid userId)
  {
    var completedWorkouts = await _context.WorkoutDays
        .Include(d => d.ProgrammeWeek)
            .ThenInclude(w => w.Programme)
        .Where(d => d.IsCompleted &&
                    !d.IsRestDay &&
                    d.ProgrammeWeek.Programme.UserId == userId)
        .ToListAsync();

    var completedSets = await _context.ExerciseSets
        .Include(s => s.WorkoutExercise)
            .ThenInclude(e => e.WorkoutDay)
                .ThenInclude(d => d.ProgrammeWeek)
                    .ThenInclude(w => w.Programme)
        .Where(s => s.IsCompleted &&
                    s.WorkoutExercise.WorkoutDay.ProgrammeWeek.Programme.UserId == userId)
        .ToListAsync();

    var totalVolume = completedSets
        .Where(s => s.Weight.HasValue && s.Reps.HasValue)
        .Sum(s => (s.Weight ?? 0) * (s.Reps ?? 0));

    var totalReps = completedSets.Sum(s => s.Reps ?? 0);

    var totalDuration = TimeSpan.FromSeconds(completedSets
        .Where(s => s.DurationSeconds.HasValue)
        .Sum(s => s.DurationSeconds ?? 0));

    // Calculate streaks based on completed workout dates
    var workoutDates = completedWorkouts
        .Where(w => w.CompletedDate.HasValue)
        .Select(w => w.CompletedDate!.Value.Date)
        .Distinct()
        .OrderBy(d => d)
        .ToList();

    var (currentStreak, longestStreak) = CalculateStreaks(workoutDates);

    return new OverallStats(
        completedWorkouts.Count,
        completedSets.Count,
        totalReps,
        totalVolume,
        totalDuration,
        currentStreak,
        longestStreak);
  }

  public async Task<IEnumerable<IntensityTrend>> GetIntensityTrendsAsync(Guid programmeId)
  {
    var weeklyMetrics = await GetWeeklyProgressAsync(programmeId);
    var metricsList = weeklyMetrics.ToList();

    var trends = new List<IntensityTrend>();

    for (int i = 0; i < metricsList.Count; i++)
    {
      var current = metricsList[i];
      var trend = "stable";

      if (i > 0)
      {
        var previous = metricsList[i - 1];
        var diff = current.AverageIntensity - previous.AverageIntensity;

        if (diff > 0.3m) trend = "increasing";
        else if (diff < -0.3m) trend = "decreasing";
      }

      var avgDifficulty = await _context.ExerciseSets
          .Include(s => s.WorkoutExercise)
              .ThenInclude(e => e.WorkoutDay)
                  .ThenInclude(d => d.ProgrammeWeek)
          .Where(s => s.IsCompleted &&
                     s.Difficulty.HasValue &&
                     s.WorkoutExercise.WorkoutDay.ProgrammeWeek.ProgrammeId == programmeId &&
                     s.WorkoutExercise.WorkoutDay.ProgrammeWeek.WeekNumber == current.WeekNumber)
          .Select(s => (int)s.Difficulty!.Value)
          .DefaultIfEmpty(0)
          .AverageAsync();

      trends.Add(new IntensityTrend(
          current.WeekNumber,
          current.AverageIntensity,
          (decimal)avgDifficulty,
          trend));
    }

    return trends;
  }

  public async Task<IEnumerable<VolumeComparison>> GetVolumeComparisonAsync(Guid programmeId)
  {
    var weeklyMetrics = (await GetWeeklyProgressAsync(programmeId)).ToList();
    var comparisons = new List<VolumeComparison>();

    for (int i = 0; i < weeklyMetrics.Count; i++)
    {
      var current = weeklyMetrics[i];
      var percentageChange = 0m;

      if (i > 0 && weeklyMetrics[i - 1].TotalVolume > 0)
      {
        var previous = weeklyMetrics[i - 1].TotalVolume;
        percentageChange = ((current.TotalVolume - previous) / previous) * 100;
      }

      comparisons.Add(new VolumeComparison(
          current.WeekNumber,
          current.TotalVolume,
          Math.Round(percentageChange, 1)));
    }

    return comparisons;
  }

  private static (int currentStreak, int longestStreak) CalculateStreaks(List<DateTime> sortedDates)
  {
    if (sortedDates.Count == 0) return (0, 0);

    var today = DateTime.UtcNow.Date;
    var currentStreak = 0;
    var longestStreak = 1;
    var tempStreak = 1;

    // Check if user worked out today or yesterday for current streak
    if (sortedDates.Contains(today) || sortedDates.Contains(today.AddDays(-1)))
    {
      currentStreak = 1;
      var checkDate = sortedDates.Contains(today) ? today : today.AddDays(-1);

      for (int i = sortedDates.IndexOf(checkDate) - 1; i >= 0; i--)
      {
        if ((checkDate - sortedDates[i]).Days <= 2) // Allow 1 day gap
        {
          currentStreak++;
          checkDate = sortedDates[i];
        }
        else break;
      }
    }

    // Calculate longest streak
    for (int i = 1; i < sortedDates.Count; i++)
    {
      if ((sortedDates[i] - sortedDates[i - 1]).Days <= 2)
      {
        tempStreak++;
        longestStreak = Math.Max(longestStreak, tempStreak);
      }
      else
      {
        tempStreak = 1;
      }
    }

    return (currentStreak, longestStreak);
  }
}

