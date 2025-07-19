using Ardalis.Result;
using Contracts.Commands;
using QuestionService.Application.DTO;
using QuestionService.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionService.Application.Features.GetQuestions;

public class GetQuestionsHandler : 
    ICommandHandler<PagedResult<IEnumerable<QuestionShortDTO>>, GetQuestionsCommand> {

    private readonly IQuestionServiceRepository _questionServiceRepository;

    public GetQuestionsHandler( IQuestionServiceRepository questionServiceRepository ) {
        _questionServiceRepository = questionServiceRepository;
    }

    public async Task<Result<PagedResult<IEnumerable<QuestionShortDTO>>>> Handle( GetQuestionsCommand command, CancellationToken cancellationToken ) {

        var result = 
            await _questionServiceRepository.GetQuestionsAsync(command.PageParams, command.SortParams, cancellationToken);

        if(!result.IsSuccess)
            return Result.Error(new ErrorList(result.Errors));  

        IEnumerable<QuestionShortDTO> questionShorts = result.Value.items
            .Select(i => new QuestionShortDTO {
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
            .Success(new PagedResult<IEnumerable<QuestionShortDTO>>(result.Value.pageInfo, questionShorts));

    }

}
