namespace Contracts.Publishers.VoteService;

public record QuestionVoted(
  Guid SourceEventId,
  Guid ParentId,
  Guid QuestionId,
  Guid OwnerUserId,
  Guid AuthorUserId,
  VoteKind ValueOldKind,
  VoteKind ValueNewKind,
  int Version,
  DateTime OccurredAt);
