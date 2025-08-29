using AnswerService.Domain.Entities;

using Ardalis.Result;

namespace AnswerService.Domain.Repositories;

public interface IAnswerChangingHistoryRepository {

  Task<Result> CreateAsync (AnswerChangingHistory  answerChangingHistory, CancellationToken ct);
  Task<Result> DeleteAsync (Guid editedUserId, CancellationToken ct);
    Task<Result<IEnumerable<AnswerChangingHistory>>> GetByAnswerIdAsync( Guid answerId, CancellationToken ct );
}