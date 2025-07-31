namespace Contracts.AnswerService.Requests;

public record DeleteAnswerRequest (Guid QuestionId, List<int> TagIds);