namespace trAInr.Application.DTOs.AI;

public class AiProgramStructure
{
    public string ProgramName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<AiWeekStructure> Weeks { get; set; } = [];
}