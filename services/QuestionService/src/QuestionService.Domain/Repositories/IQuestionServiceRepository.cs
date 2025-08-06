using Ardalis.Result;

using Contracts.Common.Filters;

using QuestionService.Domain.Entities;

namespace QuestionService.Domain.Repositories;

public interface IQuestionServiceRepository {
    Task<Result<(IEnumerable<Question> items, PagedInfo pageInfo)>> GetQuestionsAsync( 
        PageParams pageParams, 
        SortParams sortParams, 
        TagFilter tagFilter,
        CancellationToken token );

    Task<Result<(IEnumerable<Question> items, PagedInfo pageInfo)>> GetUserQuestionsAsync( 
        Guid userId,
        PageParams pageParams,
        SortParams sortParams,
        CancellationToken token );

    Task<Result<Question>> GetQuestionAsync(Guid questionId, CancellationToken token );
    Task<Result<Question>> GetQuestionShortAsync( Guid questionId, CancellationToken token );
    
    Task<Result<IEnumerable<QuestionChangingHistory>>> GetQuestionChangingHistoryAsync(Guid questionId, CancellationToken token );
    Task<Result<IEnumerable<QuestionTag>>> GetQuestionTagsAsync( Guid questionId, CancellationToken token );


    Task<Result> CreateQuestionAsync(Question question, CancellationToken token );    
    Task<Result> CreateQuestionChangingHistoryAsync( QuestionChangingHistory questionChangingHistory, CancellationToken cancellationToken );

    Task<Result> UpdateQuestionAsync( Question question, CancellationToken token );
    Task<Result> UpdateQuestionTagsAsync( Guid questionId, List<QuestionTag> questionTags, CancellationToken token );
    Task<Result> DeleteQuestionAsync( Guid questionId, CancellationToken token );
    Task SaveChangesAsync(CancellationToken token);
}
