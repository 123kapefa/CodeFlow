namespace Contracts.Publishers.VoteService;

public record AnswerVoted(
  Guid SourceEventId,
  Guid ParentId,
  Guid AnswerId,
  Guid OwnerUserId,
  Guid AuthorUserId,
  VoteKind ValueOldKind,
  VoteKind ValueNewKind,
  int Version,
  DateTime OccurredAt);