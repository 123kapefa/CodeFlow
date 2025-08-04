namespace Contracts.Requests.AnswerService;

public record UpdateAnswerRequest (Guid EditedUserId, string Content);