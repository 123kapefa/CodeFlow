namespace Contracts.Publishers.AnswerService;

public record AnswerAccepted (Guid QuestionId, Guid AnswerId, Guid UserAnswerId, int ReputationValue);