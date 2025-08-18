using ReputationService.Domain.Policies;

namespace ReputationService.Domain.Entities;

public class ReputationEntry {

  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public Guid SourceId { get; set; }
  public ReputationSourceType SourceType { get; set; }
  public int Delta { get; set; }
  public ReasonCode ReasonCode { get; set; }
  public string ReasonDetails { get; set; } = null!;
  public DateTimeOffset OccurredAt { get; set; }

  public Guid SourceEventId { get; private set; }
  public string SourceService { get; private set; } = null!;

  protected ReputationEntry () { }

  private ReputationEntry (
    Guid userId,
    Guid sourceId,
    ReputationSourceType sourceType,
    int delta,
    ReasonCode reasonCode,
    DateTimeOffset occurredAt,
    Guid sourceEventId,
    string sourceService) {
    if (delta == 0) throw new ArgumentOutOfRangeException(nameof(delta), "Delta must be non-zero.");
    Id = Guid.NewGuid ();
    UserId = userId;
    SourceId = sourceId;
    SourceType = sourceType;
    Delta = delta;
    ReasonCode = reasonCode;
    OccurredAt = occurredAt;
    SourceEventId = sourceEventId;
    SourceService = sourceService;
  }

  public static ReputationEntry Create (
    Guid userId,
    Guid sourceId,
    ReputationSourceType sourceType,
    int delta,
    ReasonCode reasonCode,
    DateTimeOffset occurredAt,
    Guid sourceEventId,
    string sourceService) =>
    new ReputationEntry (userId, 
      sourceId, 
      sourceType, 
      delta, 
      reasonCode, 
      occurredAt, 
      sourceEventId, 
      sourceService);

  public static ReputationEntry CreateOwnerChange (
    Guid ownerUserId,
    Guid sourceId,
    ReputationSourceType sourceType,
    ReasonCode reasonCode,
    int delta,
    DateTimeOffset occurredAt) {
    var e = new ReputationEntry {
      Id = Guid.NewGuid (),
      UserId = ownerUserId,
      SourceId = sourceId,
      SourceType = sourceType,
      ReasonCode = reasonCode,
      Delta = delta,
      OccurredAt = occurredAt,
      ReasonDetails = ReputationReasonText.For (sourceType, reasonCode)
    };

    ValidateDelta (reasonCode, delta);
    return e;
  }

  public static ReputationEntry CreateVoterPenalty (
    Guid voterUserId,
    Guid sourceId,
    ReputationSourceType sourceType,
    DateTimeOffset occurredAt) =>
    CreateOwnerChange (voterUserId, sourceId, sourceType, ReasonCode.Downvote, ReputationPoints.VoterDownvotePenalty,
      occurredAt);

  private static void ValidateDelta (ReasonCode reason, int delta) {
    switch (reason) {
      case ReasonCode.Upvote when delta <= 0:
      case ReasonCode.AcceptedAnswer when delta <= 0:
        throw new ArgumentException ($"Delta must be positive for {reason}");
      case ReasonCode.Downvote when delta >= 0:
      case ReasonCode.Delete when delta >= 0:
        throw new ArgumentException ($"Delta must be negative for {reason}");
      default:
        break;
    }
  }

}

public enum ReputationSourceType {

  Question = 1,
  Answer = 2

}

public enum ReasonCode {

  Upvote = 1,
  Downvote = 2,
  AcceptedAnswer = 3,
  Comment = 4,
  Delete = 5,

}