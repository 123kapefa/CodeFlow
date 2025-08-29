namespace ReputationService.Domain.Entities;

public sealed class ReputationSummary {

  public Guid UserId { get; set; }
  public int Total { get; set; }
  public long Version { get; set; }
  public DateTime UpdatedAt { get; set; }

}