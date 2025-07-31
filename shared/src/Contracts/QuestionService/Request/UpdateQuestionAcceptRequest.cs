namespace Contracts.QuestionService.Request;

public record UpdateQuestionAcceptRequest (Guid AcceptAnswerId, Guid UserAnswerId);