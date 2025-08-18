namespace ReputationService.Domain.Entities;

public class ReputationSummary {

  public Guid UserId { get; set; }
  public int Total { get; set; }
  public DateTime UpdatedAt { get; set; }

  protected ReputationSummary () { }
  
  private ReputationSummary (Guid userId) {
    UserId = userId;
    Total = 0;
    UpdatedAt = DateTime.UtcNow;
  }

  public static ReputationSummary Create (Guid userId) => new ReputationSummary (userId);

}