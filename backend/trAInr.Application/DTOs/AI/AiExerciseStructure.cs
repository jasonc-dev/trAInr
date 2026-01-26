namespace trAInr.Application.DTOs.AI;

public class AiExerciseStructure
{
  public int ExerciseDefinitionId { get; set; }
  public int OrderIndex { get; set; }
  public int TargetSets { get; set; }
  public int TargetReps { get; set; }
  public decimal? TargetWeight { get; set; }
  public int? TargetDurationSeconds { get; set; }
  public decimal? TargetDistance { get; set; }
  public int? RestSeconds { get; set; }
  public Guid? SupersetGroupId { get; set; }
  public int? SupersetRestSeconds { get; set; }
  public int? TargetRpe { get; set; }
  public string? Notes { get; set; }
}