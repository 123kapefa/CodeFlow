namespace QuestionService.Application.Abstractions;

public interface IQuestionViewTracker {

  Task<bool> TryTrackAsync(Guid questionId, string viewerKey, CancellationToken ct);

}