namespace Contracts.Publishers.AnswerService;

public record AnswerVoted (Guid UserId, int ReputationValue);