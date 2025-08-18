namespace Contracts.Publishers.AnswerService;

public record AnswerAccepted (
  Guid EventId,
  DateTime OccurredAt,
  Guid QuestionId,
  Guid? OldAnswerId,
  Guid? OldAnswerOwnerUserId,
  Guid? NewAnswerId,
  Guid? NewAnswerOwnerUserId,
  int Version,   
  string? CorrelationId = null);