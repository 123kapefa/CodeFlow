using Ardalis.Result;
using QuestionService.Domain.Repositories;

using Abstractions.Commands;

using Contracts.DTOs.QuestionService;

namespace QuestionService.Application.Features.GetUserQuestions;

public class GetUserQuestionsHandler :
    ICommandHandler<PagedResult<IEnumerable<QuestionShortDTO>>, GetUserQuestionsCommand> {

    private readonly IQuestionServiceRepository _questionServiceRepository;

    public GetUserQuestionsHandler( IQuestionServiceRepository questionServiceRepository ) {
        _questionServiceRepository = questionServiceRepository;
    }

    public async Task<Result<PagedResult<IEnumerable<QuestionShortDTO>>>> Handle( 
        GetUserQuestionsCommand command, 
        CancellationToken cancellationToken ) {

        var result = 
            await _questionServiceRepository.GetUserQuestionsAsync(
                command.UserId, command.PageParams, command.SortParams, cancellationToken);

        if(!result.IsSuccess)
            return Result.Error(new ErrorList(result.Errors));

        IEnumerable<QuestionShortDTO> userQuestions = result.Value.items.Select(i => new QuestionShortDTO {
            Id = i.Id,
            UserId = i.UserId,
            Title = i.Title,
            Content = i.Content,
            CreatedAt = i.CreatedAt,
            ViewsCount = i.ViewsCount,
            AnswersCount = i.AnswersCount,
            Upvotes = i.Upvotes,
            Downvotes = i.Downvotes,
            IsClosed = i.IsClosed
        }).ToList();

        return Result<PagedResult<IEnumerable<QuestionShortDTO>>>
            .Success(new PagedResult<IEnumerable<QuestionShortDTO>>(result.Value.pageInfo, userQuestions));
    }
}
