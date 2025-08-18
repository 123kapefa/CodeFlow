namespace ReputationService.Domain.Entities;

public sealed class ReputationEffect {

  public Guid Id { get; private set; }
  public Guid UserId { get; private set; }
  public Guid SourceId { get; private set; }
  public ReputationSourceType SourceType { get; private set; }
  public string EffectKind { get; private set; } = null!; // "VoteOwner" | "VoterPenalty" | "AcceptedAnswer"
  public int Amount { get; private set; } // текущее влияние (+10/-2/-1/+15/0)
  public int LastVersion { get; private set; } // защита от out-of-order
  public Guid LastEventId { get; private set; } // идемпотентность
  public DateTime UpdatedAt { get; private set; }
  public string SourceService { get; private set; } = null!;
  public string? CorrelationId { get; private set; }

  private ReputationEffect () { }

  public static ReputationEffect Create (
    Guid userId,
    Guid sourceId,
    ReputationSourceType st,
    string effectKind,
    int amount,
    int version,
    Guid eventId,
    string sourceService,
    string? correlationId) =>
    new () {
      Id = Guid.NewGuid (),
      UserId = userId,
      SourceId = sourceId,
      SourceType = st,
      EffectKind = effectKind,
      Amount = amount,
      LastVersion = version,
      LastEventId = eventId,
      UpdatedAt = DateTime.UtcNow,
      SourceService = sourceService,
      CorrelationId = correlationId
    };

  public (int Delta, bool Applied) Apply (int newAmount, int version, Guid eventId, string? correlationId) {
    if (eventId == LastEventId || version <= LastVersion) return (0, false);
    var delta = newAmount - Amount;
    if (delta == 0) {
      LastVersion = version;
      LastEventId = eventId;
      CorrelationId = correlationId;
      return (0, false);
    }

    Amount = newAmount;
    LastVersion = version;
    LastEventId = eventId;
    UpdatedAt = DateTime.UtcNow;
    CorrelationId = correlationId;
    return (delta, true);
  }

}