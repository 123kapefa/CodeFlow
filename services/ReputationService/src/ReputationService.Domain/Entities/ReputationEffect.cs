namespace ReputationService.Domain.Entities;

public sealed class ReputationEffect {

  public Guid Id { get; private set; }
  public Guid UserId { get; private set; }
  public Guid ParentId { get; private set; }
  public Guid SourceId { get; private set; }
  public Guid LastEventId { get; private set; }
  public string SourceService { get; private set; } = null!;
  public ReputationSourceType SourceType { get; private set; }
  public string EffectKind { get; private set; } = null!;
  public int Amount { get; private set; }
  public int LastVersion { get; private set; }
  public DateTime UpdatedAt { get; private set; }

  private ReputationEffect () { }

  public static ReputationEffect Create (
    Guid userId,
    Guid parentId,
    Guid sourceId,
    ReputationSourceType st,
    string effectKind,
    int amount,
    int version,
    Guid eventId,
    string sourceService) =>
    new () {
      Id = Guid.NewGuid (),
      UserId = userId,
      ParentId = parentId,
      SourceId = sourceId,
      SourceType = st,
      EffectKind = effectKind,
      Amount = amount,
      LastVersion = version,
      LastEventId = eventId,
      UpdatedAt = DateTime.UtcNow,
      SourceService = sourceService
    };

  public bool Apply (int delta, Guid eventId) {
    if (eventId == LastEventId || delta == 0) return (false);

    Amount += delta;
    LastEventId = eventId;
    UpdatedAt = DateTime.UtcNow;
    return (true);
  }

}