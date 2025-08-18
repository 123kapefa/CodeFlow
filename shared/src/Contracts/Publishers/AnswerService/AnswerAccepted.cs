namespace Contracts.Publishers.AnswerService;

public record AnswerAccepted (
  Guid EventId,
  DateTimeOffset OccurredAt,
  Guid QuestionId,
  Guid? OldAnswerId,
  Guid? OldAnswerOwnerUserId,
  Guid? NewAnswerId,
  Guid? NewAnswerOwnerUserId,
  string? CorrelationId = null);