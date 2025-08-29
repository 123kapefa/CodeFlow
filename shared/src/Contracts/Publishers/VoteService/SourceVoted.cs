namespace Contracts.Publishers.VoteService;

public record SourceVoted (
  Guid EventId,
  DateTime OccurredAt,
  Guid QuestionId,
  Guid? OldAnswerId,
  Guid? OldAnswerOwnerUserId,
  Guid? NewAnswerId,
  Guid? NewAnswerOwnerUserId);