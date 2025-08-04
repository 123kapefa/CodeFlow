using Ardalis.Result;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Repositories;

using Abstractions.Commands;

using Contracts.DTOs.QuestionService;

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
            QuestionChangingHistories = 
            result.Value.QuestionChangingHistories
            .Select(qh => new QuestionHistoryDTO {
                UserId = qh.Id,
                Content = qh.Content,
                UpdatedAt = qh.UpdatedAt                
            })
            .ToList(),
            QuestionTags = 
            result.Value.QuestionTags
            .Select(qt => new QuestionTagDTO {
                TagId = qt.TagId,
                WatchedAt = qt.WatchedAt
            })
            .ToList()
        };

        return Result<QuestionDTO>.Success(question) ;
    }
}
