namespace Contracts.Publishers.VoteService;

public record QuestionVotedEvent (
  Guid SourceEventId,
  Guid QuestionId,
  Guid OwnerUserId,
  Guid AuthorUserId,
  VoteKind ValueKind,
  int Version,
  DateTime OccurredAt);