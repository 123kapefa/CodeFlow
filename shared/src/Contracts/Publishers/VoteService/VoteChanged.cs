namespace Contracts.Publishers.VoteService;

public enum VotableSourceType { Question, Answer, Comment }
public enum VoteKind { None = 0, Up = 1, Down = -1 }

public sealed record VoteChanged(
  Guid EventId,
  DateTime OccurredAt,
  VotableSourceType SourceType,
  Guid SourceId,
  Guid SourceOwnerUserId,
  Guid AuthorUserId,
  VoteKind? OldKind,
  VoteKind? NewKind
);