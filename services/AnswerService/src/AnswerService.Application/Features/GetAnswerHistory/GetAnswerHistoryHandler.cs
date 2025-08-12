using Abstractions.Commands;
using AnswerService.Domain.Repositories;
using Ardalis.Result;
using Contracts.DTOs.AnswerService;

namespace AnswerService.Application.Features.GetAnswerHistory;

public class GetAnswerHistoryHandler : ICommandHandler<IEnumerable<AnswerHistoryDTO>, GetAnswerHistoryCommand> {

    private readonly IAnswerChangingHistoryRepository _answerChangingHistoryRepository;

    public GetAnswerHistoryHandler( IAnswerChangingHistoryRepository answerChangingHistoryRepository ) {
        _answerChangingHistoryRepository = answerChangingHistoryRepository;
    }

    public async Task<Result<IEnumerable<AnswerHistoryDTO>>> Handle( GetAnswerHistoryCommand command, CancellationToken token ) {
        if(command.answerId == Guid.Empty)
            return Result<IEnumerable<AnswerHistoryDTO>>.Error("Answer ID cannot be empty.");

        Result<IEnumerable<Domain.Entities.AnswerChangingHistory>> history = await _answerChangingHistoryRepository.GetByAnswerIdAsync(command.answerId, token);


        if(!history.IsSuccess)
            return Result<IEnumerable<AnswerHistoryDTO>>.Error(new ErrorList(history.Errors));

        IEnumerable<AnswerHistoryDTO> historyDto = history.Value.Select(h => new AnswerHistoryDTO(
            h.AnswerId, h.UserId, h.Content, h.UpdatedAt));

        return Result<IEnumerable<AnswerHistoryDTO>>.Success(historyDto);
    }
}

  
