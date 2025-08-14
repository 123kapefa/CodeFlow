using AnswerService.Domain.Entities;

using Ardalis.Result;

using Contracts.Common.Filters;

namespace AnswerService.Domain.Repositories;

public interface IAnswerRepository {

  Task<Result<IEnumerable<Answer>>> GetByQuestionIdAsync (Guid questionId, CancellationToken ct);
  Task<Result<Answer>> GetByIdAsync (Guid id, CancellationToken ct);
  Task<Result> CreateAsync (Answer answer, AnswerChangingHistory answerChangingHistory, CancellationToken ct);
  Task<Result> DeleteAsync (Answer answer, CancellationToken ct);
  Task<Result> DeleteAsync (IEnumerable<Answer> answers, CancellationToken ct);
  Task<Result> UpdateAsync (Answer answer, CancellationToken ct);
  Task<Result> UpdateAsync (Answer answer, AnswerChangingHistory answerChangingHistory, CancellationToken ct);
  Task<Result> AcceptAsync (IEnumerable<Answer> answers, Guid answerId, CancellationToken ct);

  Task<Result<(IEnumerable<Answer> items, PagedInfo pageInfo)>> GetByUserIdAsync (
    Guid userId,
    PageParams pageParams,
    SortParams sortParams,
    CancellationToken ct);

  Task<Result<IEnumerable<Answer>>> GetByUserIdAsync (Guid userId, CancellationToken ct);
  Task<Result<IEnumerable<string>>> GetCommentsByAnswerIdAsync (Guid userId, CancellationToken ct);

  Task<Result<IEnumerable<Guid>>> GetQuestionIdsByUserId (
    Guid userId,
    PageParams pageParams,
    SortParams sortParams,
    CancellationToken ct);

  Task SaveAsync (CancellationToken ct);

}