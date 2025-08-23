using Abstractions.Commands;

using AnswerService.Domain.Repositories;

using Ardalis.Result;

using Contracts.Publishers.AnswerService;
using Contracts.Publishers.VoteService;

using Messaging.Broker;

namespace AnswerService.Application.Features.UpdateAnswerVote;

public class UpdateAnswerVoteHandler : ICommandHandler<UpdateAnswerVoteCommand> {

  private readonly IAnswerRepository _answerRepository;

  public UpdateAnswerVoteHandler (
    IAnswerRepository answerRepository) {
    _answerRepository = answerRepository;
  }

  public async Task<Result> Handle (UpdateAnswerVoteCommand command, CancellationToken cancellationToken) {
    if (command.AnswerId == Guid.Empty)
      return Result.Error ("ID ответа не может быть пустым.");
    
    if(command.VoteValue == VoteKind.None)
      return Result.Error("Допустимые значения: 1 или -1");
    
    var answerResult = await _answerRepository.GetByIdAsync (command.AnswerId, cancellationToken);
    
    if (!answerResult.IsSuccess)
      return Result.Error (new ErrorList(answerResult.Errors));

    switch (command.VoteValue) {
      case VoteKind.Up: {
        answerResult.Value.Upvotes += 1;
        break;
      }
      case VoteKind.Down: {
        answerResult.Value.Downvotes += 1;
        break;
      }
      case VoteKind.None: {
        break;
      }
    }
    
    var updateResult = await _answerRepository.UpdateAsync (answerResult.Value, cancellationToken);
    
    if (!updateResult.IsSuccess)
      return Result.Error (new ErrorList(updateResult.Errors));
    
    await _answerRepository.SaveAsync(cancellationToken);
    
    return Result.Success ();
  }

}