namespace trAInr.Application.DTOs.AI;

public class AiWeekStructure
{
  public int WeekNumber { get; set; }
  public string? Notes { get; set; }
  public List<AiWorkoutDayStructure> WorkoutDays { get; set; } = [];
}