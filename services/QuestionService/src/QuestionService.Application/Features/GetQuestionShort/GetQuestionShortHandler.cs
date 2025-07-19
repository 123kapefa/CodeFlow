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

namespace QuestionService.Application.Features.GetQuestionShort;

public class GetQuestionShortHandler : ICommandHandler<QuestionShortDTO, GetQuestionShortCommand> {

    private readonly IQuestionServiceRepository _questionServiceRepository;

    public GetQuestionShortHandler( IQuestionServiceRepository questionServiceRepository ) {
        _questionServiceRepository = questionServiceRepository;
    }

    public async Task<Result<QuestionShortDTO>> Handle( GetQuestionShortCommand command, CancellationToken cancellationToken ) {

        Result<Question> result = await _questionServiceRepository.GetQuestionShortAsync(command.questionId, cancellationToken);

        if(!result.IsSuccess)
            return Result<QuestionShortDTO>.Error(new ErrorList(result.Errors));

        QuestionShortDTO question = new QuestionShortDTO {
            Id = result.Value.Id,
            UserId = result.Value.UserId,
            UserEditorId = result.Value.UserEditorId,
            Title = result.Value.Title,
            Content = result.Value.Content,
            CreatedAt = result.Value.CreatedAt,
            UpdatedAt = result.Value.UpdatedAt,
            ViewsCount = result.Value.ViewsCount,
            AnswersCount = result.Value.AnswersCount,
            Upvotes = result.Value.Upvotes,
            Downvotes = result.Value.Downvotes,
            IsClosed = result.Value.IsClosed,
            AcceptedAnswerId = result.Value.AcceptedAnswerId
        };


        return Result<QuestionShortDTO>.Success(question);
    }
}
