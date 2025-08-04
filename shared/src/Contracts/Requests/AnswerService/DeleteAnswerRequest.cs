namespace Contracts.Requests.AnswerService;

public record DeleteAnswerRequest (Guid QuestionId, List<int> TagIds);