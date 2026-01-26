using System.Text.Json.Serialization;

namespace trAInr.Application.DTOs.AI;

public class AiExerciseStructure
{
    [JsonConverter(typeof(StringToIntConverter))]
    public int ExerciseDefinitionId { get; set; }
    
    [JsonConverter(typeof(StringToIntConverter))]
    public int OrderIndex { get; set; }
    
    [JsonConverter(typeof(StringToNullableIntConverter))]
    public int? TargetSets { get; set; }
    
    [JsonConverter(typeof(StringToNullableIntConverter))]
    public int? TargetReps { get; set; }
    
    public decimal? TargetWeight { get; set; }
    
    [JsonConverter(typeof(StringToNullableIntConverter))]
    public int? TargetDurationSeconds { get; set; }
    
    public decimal? TargetDistance { get; set; }
    
    [JsonConverter(typeof(StringToNullableIntConverter))]
    public int? RestSeconds { get; set; }
    
    public Guid? SupersetGroupId { get; set; }
    
    [JsonConverter(typeof(StringToNullableIntConverter))]
    public int? SupersetRestSeconds { get; set; }
    
    [JsonConverter(typeof(StringToNullableIntConverter))]
    public int? TargetRpe { get; set; }
    
    public string? Notes { get; set; }
}