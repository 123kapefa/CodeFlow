namespace AnswerService.Application.Requests;

public record CreateAnswerRequest (Guid QuestionId, Guid UserId, string Content);