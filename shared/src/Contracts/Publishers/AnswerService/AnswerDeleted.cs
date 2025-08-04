namespace Contracts.Publishers.AnswerService;

public record AnswerDeleted (Guid UserId, Guid QuestionId, Guid AnswerId, List<int> TagIds);