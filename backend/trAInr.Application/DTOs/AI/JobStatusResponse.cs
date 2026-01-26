using trAInr.Application.DTOs.ProgramTemplate;

namespace trAInr.Application.DTOs.AI;

public class JobStatusResponse
{
    public Guid JobId { get; set; }
    public string Status { get; set; } = string.Empty; // Pending, Processing, Completed, Failed
    public ProgramTemplateResponse? Result { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
