using Contracts.Publishers.VoteService;

namespace VoteService.Domain.Entities;

public sealed class Vote {

  public Guid Id { get; private set; }
  public Guid AuthorUserId { get; private set; }
  public Guid SourceId { get; private set; }
  public VotableSourceType SourceType { get; private set; }
  public VoteKind Kind { get; private set; }
  public int Version { get; private set; } = 1;
  public DateTime ChangedAt { get; private set; }

  private Vote () { }

  public static Vote Create (Guid authorUserId, Guid sourceId, VotableSourceType type, VoteKind kind) =>
    new () {
      Id = Guid.NewGuid (),
      AuthorUserId = authorUserId,
      SourceId = sourceId,
      SourceType = type,
      Kind = kind,
      ChangedAt = DateTime.UtcNow
    };

  public (VoteKind Old, VoteKind New) Set (VoteKind newKind) {
    if (newKind == Kind) return (Kind, Kind);
    var old = Kind;
    Kind = newKind;
    ChangedAt = DateTime.UtcNow;
    Version++;
    return (old, newKind);
  }

}