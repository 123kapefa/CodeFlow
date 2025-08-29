namespace Contracts.Publishers.AnswerService;

public record AnswerAccepted (
  Guid EventId,
  Guid ParentId,
  Guid? OldAnswerId,
  Guid? OldAnswerOwnerUserId,
  Guid NewAnswerId,
  Guid NewAnswerOwnerUserId,
  int Version,
  DateTime OccurredAt);