namespace Contracts.AnswerService.Requests;

public record UpdateAnswerRequest (Guid EditedUserId, string Content);