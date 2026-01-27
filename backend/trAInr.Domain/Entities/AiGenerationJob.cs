namespace trAInr.Domain.Entities;

public class AiGenerationJob
{
    public Guid Id { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Processing, Completed, Failed
    public string RequestData { get; set; } = string.Empty; // JSON serialized GenerateProgramRequest
    public string? ResultData { get; set; } // JSON serialized ProgramTemplateResponse (when completed)
    public string? ErrorMessage { get; set; }
    public Guid? ProgramTemplateId { get; set; } // Reference to created program template
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
