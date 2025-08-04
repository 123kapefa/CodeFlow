namespace Contracts.Requests.QuestionService;

public record UpdateQuestionAcceptRequest (Guid AcceptAnswerId, Guid UserAnswerId);