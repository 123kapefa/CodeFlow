namespace AnswerService.Application.Requests;

public record UpdateAnswerRequest (Guid EditedUserId, string Content);