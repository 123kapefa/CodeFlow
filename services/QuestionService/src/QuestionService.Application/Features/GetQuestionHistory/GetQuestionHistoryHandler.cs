using Ardalis.Result;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Repositories;

using Abstractions.Commands;

using Contracts.DTOs.QuestionService;

namespace QuestionService.Application.Features.GetQuestionHistory;

public class GetQuestionHistoryHandler : ICommandHandler<IEnumerable<QuestionHistoryDTO>, GetQuestionHistoryCommand> {

    private readonly IQuestionServiceRepository _questionServiceRepository;

    public GetQuestionHistoryHandler( IQuestionServiceRepository questionServiceRepository ) {
        _questionServiceRepository = questionServiceRepository;
    }

    public async Task<Result<IEnumerable<QuestionHistoryDTO>>> Handle( GetQuestionHistoryCommand command, CancellationToken cancellationToken ) {

        Result<IEnumerable<QuestionChangingHistory>> result = 
            await _questionServiceRepository.GetQuestionChangingHistoryAsync(command.questionId, cancellationToken);

        if(!result.IsSuccess)
            return Result<IEnumerable<QuestionHistoryDTO>>.Error(new ErrorList(result.Errors));

        List <QuestionHistoryDTO> questionHistories = result.Value
            .Select(r => new QuestionHistoryDTO {
                UserId = r.UserId,
                Content = r.Content,
                UpdatedAt = r.UpdatedAt
        }).ToList();

        return Result<IEnumerable<QuestionHistoryDTO>>.Success(questionHistories);
    }
}
