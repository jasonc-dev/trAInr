namespace trAInr.Application.DTOs.AI;

public class AiWorkoutDayStructure
{
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }
  public bool IsRestDay { get; set; }
  public List<AiExerciseStructure> Exercises { get; set; } = [];
}