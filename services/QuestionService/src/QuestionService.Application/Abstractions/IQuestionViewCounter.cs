namespace QuestionService.Application.Abstractions;

public interface IQuestionViewCounter {

  Task IncrementAsync(Guid questionId, CancellationToken ct);

}