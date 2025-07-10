using Ardalis.Result;
using Contracts.Commands;
using QuestionService.Application.DTO;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionService.Application.Features.GetQuestionTags;

public class GetQuestionTagsHandler : ICommandHandler<IEnumerable<QuestionTagDTO>, GetQuestionTagsCommand> {

    private readonly IQuestionServiceRepository _questionServiceRepository;

    public GetQuestionTagsHandler( IQuestionServiceRepository questionServiceRepository ) {
        _questionServiceRepository = questionServiceRepository;
    }

    public async Task<Result<IEnumerable<QuestionTagDTO>>> Handle( GetQuestionTagsCommand command, CancellationToken cancellationToken ) {

        Result<IEnumerable<QuestionTag>> result = 
            await _questionServiceRepository.GetQuestionTagsAsync(command.questionId, cancellationToken);

        if(!result.IsSuccess)
            return Result<IEnumerable<QuestionTagDTO>>.Error(new ErrorList(result.Errors));

        List<QuestionTagDTO> questionTags = result.Value
            .Select(qt => new QuestionTagDTO {
                Id = qt.Id,
                TagId = qt.TagId,
                WatchedAt = qt.WatchedAt
            })
            .ToList();

        return Result<IEnumerable<QuestionTagDTO>>.Success(questionTags);
    }
}
