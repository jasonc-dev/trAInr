namespace trAInr.API.Models.Domain;

/// <summary>
/// Represents a workout programme spanning 4-10 weeks
/// </summary>
public class Programme
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public int DurationWeeks { get; set; }
  public bool IsPreMade { get; set; }
  public bool IsActive { get; set; }
  public DateTime StartDate { get; set; }
  public DateTime? EndDate { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

  // Navigation properties
  public User User { get; set; } = null!;
  public ICollection<ProgrammeWeek> Weeks { get; set; } = new List<ProgrammeWeek>();
}

