namespace ReputationService.Domain.Entities;

public class ReputationSummary {

  public Guid UserId { get; set; }
  public int Total { get; set; }

  protected ReputationSummary () { }
  
  private ReputationSummary (Guid userId) {
    UserId = userId;
    Total = 0;
  }

  public static ReputationSummary Create (Guid userId) => new ReputationSummary (userId);

}