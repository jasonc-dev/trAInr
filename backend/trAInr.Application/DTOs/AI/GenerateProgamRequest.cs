using trAInr.Domain.Entities;

namespace trAInr.Application.DTOs.AI;

public class GenerateProgamRequest
{
  public string ProgramName { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public int DurationWeeks { get; set; }
  public ExperienceLevel ExperienceLevel { get; set; }
  public List<string> WorkoutDayNames { get; set; } = [];
}