using ReputationService.Domain.Policies;

namespace ReputationService.Domain.Entities;

public sealed class ReputationEntry {

  public Guid Id { get; private set; }
  public Guid UserId { get; private set; }
  public Guid SourceId { get; private set; }
  public ReputationSourceType SourceType { get; private set; }
  public string EffectKind { get; private set; } = null!;
  public ReasonCode ReasonCode { get; private set; }
  public int Delta { get; private set; } // именно Δ (new - old)
  public string ReasonDetails { get; private set; } = null!;
  public DateTime OccurredAt { get; private set; }
  public Guid SourceEventId { get; private set; }
  public string SourceService { get; private set; } = null!;
  public string? CorrelationId { get; private set; }

  private ReputationEntry () { }

  public static ReputationEntry Create (
    Guid userId,
    Guid sourceId,
    ReputationSourceType st,
    string effectKind,
    ReasonCode code,
    int delta,
    DateTime occurredAt,
    Guid eventId,
    string sourceService,
    string? correlationId) =>
    new () {
      Id = Guid.NewGuid (),
      UserId = userId,
      SourceId = sourceId,
      SourceType = st,
      EffectKind = effectKind,
      ReasonCode = code,
      Delta = delta,
      ReasonDetails = $"{st}: {code}",
      OccurredAt = occurredAt,
      SourceEventId = eventId,
      SourceService = sourceService,
      CorrelationId = correlationId
    };

}