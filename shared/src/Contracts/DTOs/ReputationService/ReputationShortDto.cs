namespace Contracts.DTOs.ReputationService;

public class ReputationShortDto {

  public int Delta { get; }
  public DateOnly OccurredAt { get; }

  protected ReputationShortDto() { }
  
  private ReputationShortDto(DateOnly occurredAt, int delta) {
    Delta = delta;
    OccurredAt = occurredAt;
  }

  public static ReputationShortDto Create (
    DateOnly occurredAt, int delta) =>
    new ReputationShortDto (occurredAt, delta);

}