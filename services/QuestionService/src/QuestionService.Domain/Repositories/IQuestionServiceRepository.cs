using Ardalis.Result;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionService.Domain.Repositories;

public interface IQuestionServiceRepository {
    Task<Result<(IEnumerable<Question> items, PagedInfo pageInfo)>> GetQuestionsAsync( PageParams pageParams, SortParams sortParams, CancellationToken token );
    Task<Result<Question>> GetQuestionAsync(Guid questionId, CancellationToken token );
    Task<Result<Question>> GetQuestionShortAsync( Guid questionId, CancellationToken token );
    Task<Result<IEnumerable<Question>>> GetUserQuestionsAsync( Guid userId, CancellationToken token );
    Task<Result<IEnumerable<QuestionChangingHistory>>> GetQuestionChangingHistoryAsync(Guid questionId, CancellationToken token );
    Task<Result<IEnumerable<QuestionTag>>> GetQuestionTagsAsync( Guid questionId, CancellationToken token );

    Task<Result> CreateQuestionAsync(Question question, CancellationToken token );
    Task<Result> CreateQuestionTagsAsync(QuestionTag questionTag, CancellationToken token );
    Task<Result> CreateQuestionChangingHistoryAsync( QuestionChangingHistory questionChangingHistory, CancellationToken cancellationToken );
    Task<Result> UpdateQuestionAsync( Question question, CancellationToken token );
    Task<Result> DeleteQuestionAsync( Guid questionId, CancellationToken token );
    Task<Result> DeleteQuestionTagAsync(Guid questionId, int tagId, CancellationToken cancellationToken);

    Task<Result> UpdateQuestionAcceptAsync(Guid questionId, Guid acceptedAnswerId, Question acceptedQuestion, CancellationToken cancellationToken );
    Task<Result> UpdateQuestionViewAsync( Guid questionId, CancellationToken cancellationToken);
    Task<Result> UpdateQuestionVoteAsync( Guid questionId, int upvotes, int downvotes, CancellationToken cancellationToken );
}
