namespace Contracts.Requests.QuestionService;

public record UpdateQuestionAcceptRequest (Guid? OldAcceptedAnswerId, Guid AcceptAnswerId, Guid UserAnswerId);