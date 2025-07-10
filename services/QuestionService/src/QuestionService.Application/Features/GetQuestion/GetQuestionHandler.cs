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

namespace QuestionService.Application.Features.GetQuestion;

public class GetQuestionHandler : ICommandHandler<QuestionDTO, GetQuestionCommand> {

    private readonly IQuestionServiceRepository _questionServiceRepository;

    public GetQuestionHandler( IQuestionServiceRepository questionServiceRepository ) {
        _questionServiceRepository = questionServiceRepository;
    }

    public async Task<Result<QuestionDTO>> Handle( GetQuestionCommand command, CancellationToken cancellationToken ) {

        Result<Question> result = await _questionServiceRepository.GetQuestionAsync(command.questionId, cancellationToken);

        if(!result.IsSuccess)
            Result<QuestionDTO>.Error(new ErrorList(result.Errors));

        QuestionDTO question = new QuestionDTO { 
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
            AcceptedAnswerId = result.Value.AcceptedAnswerId,
            QuestionChangingHistories = result.Value.QuestionChangingHistories,
            QuestionTags = result.Value.QuestionTags
        };

        return Result<QuestionDTO>.Success(question) ;
    }
}
