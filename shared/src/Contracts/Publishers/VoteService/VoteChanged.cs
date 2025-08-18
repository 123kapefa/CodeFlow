namespace Contracts.Publishers.VoteService;

public enum VotableEntityType { Question, Answer, Comment }
public enum VoteKind { None = 0, Up = 1, Down = -1 }

public sealed record VoteChanged(
  Guid EventId,
  DateTime OccurredAt,
  VotableEntityType EntityType,
  Guid EntityId,
  Guid EntityOwnerUserId,
  Guid ActorUserId,
  VoteKind OldKind,
  VoteKind NewKind,
  string? CorrelationId = null
);