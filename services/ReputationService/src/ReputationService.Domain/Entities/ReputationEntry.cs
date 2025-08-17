namespace ReputationService.Domain.Entities;

public class ReputationEntry {

  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public Guid SourceId { get; set; }
  public ReputationSourceType SourceType { get; set; }
  public int Delta { get; set; }
  public string ReasonCode { get; set; } = null!;
  public string ReasonDetails { get; set; } = null!;
  public DateTimeOffset OccurredAt { get; set; }

  protected ReputationEntry () { }

  private ReputationEntry (
    Guid userId,
    Guid sourceId,
    ReputationSourceType sourceType,
    int delta,
    string reasonCode,
    string reasonDetails,
    DateTimeOffset occurredAt) {
    Id = Guid.NewGuid();
    UserId = userId;
    SourceId = sourceId;
    SourceType = sourceType;
    Delta = delta;
    ReasonCode = reasonCode;
    ReasonDetails = reasonDetails;
    OccurredAt = occurredAt;
  }

  public static ReputationEntry Create (
    Guid userId,
    Guid sourceId,
    ReputationSourceType sourceType,
    int delta,
    string reasonCode,
    string reasonDetails,
    DateTimeOffset occurredAt) =>
    new ReputationEntry (userId, sourceId, sourceType, delta, reasonCode, reasonDetails, occurredAt);

}

public enum ReputationSourceType {

  Question = 1,
  Answer = 2

}