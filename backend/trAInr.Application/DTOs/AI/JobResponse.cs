namespace trAInr.Application.DTOs.AI;

public class JobResponse
{
    public Guid JobId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
