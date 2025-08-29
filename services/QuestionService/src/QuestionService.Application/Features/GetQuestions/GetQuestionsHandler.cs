using Ardalis.Result;
using QuestionService.Domain.Repositories;

using Abstractions.Commands;

using Contracts.DTOs.QuestionService;

using QuestionService.Application.Extensions;

namespace QuestionService.Application.Features.GetQuestions;

public class GetQuestionsHandler : 
    ICommandHandler<PagedResult<IEnumerable<QuestionShortDTO>>, GetQuestionsCommand> {

    private readonly IQuestionServiceRepository _questionServiceRepository;

    public GetQuestionsHandler( IQuestionServiceRepository questionServiceRepository ) {
        _questionServiceRepository = questionServiceRepository;
    }

    public async Task<Result<PagedResult<IEnumerable<QuestionShortDTO>>>> Handle( GetQuestionsCommand command, CancellationToken cancellationToken ) {

        var result = 
            await _questionServiceRepository.GetQuestionsAsync(command.PageParams, command.SortParams, command.TagFilter, cancellationToken);

        if(!result.IsSuccess)
            return Result.Error(new ErrorList(result.Errors));

        IEnumerable<QuestionShortDTO> questionShorts = result.Value.items.ToQuestionsShortDto ();

        return Result<PagedResult<IEnumerable<QuestionShortDTO>>>
            .Success(new PagedResult<IEnumerable<QuestionShortDTO>>(result.Value.pageInfo, questionShorts));

    }

}
