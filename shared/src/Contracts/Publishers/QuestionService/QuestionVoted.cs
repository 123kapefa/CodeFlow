namespace Contracts.Publishers.QuestionService;

public record QuestionVoted (Guid UserId, int ReputationValue);
