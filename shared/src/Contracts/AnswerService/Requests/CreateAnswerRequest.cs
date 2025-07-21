namespace Contracts.AnswerService.Requests;

public record CreateAnswerRequest (Guid QuestionId, Guid UserId, string Content);