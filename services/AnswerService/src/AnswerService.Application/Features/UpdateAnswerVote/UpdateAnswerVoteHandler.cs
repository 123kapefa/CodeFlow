using Abstractions.Commands;

using AnswerService.Domain.Repositories;

using Ardalis.Result;

using Contracts.Publishers.AnswerService;

using Messaging.Broker;

namespace AnswerService.Application.Features.UpdateAnswerVote;

public class UpdateAnswerVoteHandler : ICommandHandler<UpdateAnswerVoteCommand> {

  private readonly IAnswerRepository _answerRepository;
  private readonly IMessageBroker _messageBroker;

  public UpdateAnswerVoteHandler (
    IAnswerRepository answerRepository
    , IMessageBroker messageBroker) {
    _answerRepository = answerRepository;
    _messageBroker = messageBroker;
  }

  public async Task<Result> Handle (UpdateAnswerVoteCommand command, CancellationToken cancellationToken) {
    if (command.AnswerId == Guid.Empty)
      return Result.Error ("ID ответа не может быть пустым.");
    
    if(command.VoteValue == 0)
      return Result.Error("Допустимые значения: 1 или -1");
    
    var answerResult = await _answerRepository.GetByIdAsync (command.AnswerId, cancellationToken);
    
    if (!answerResult.IsSuccess)
      return Result.Error (new ErrorList(answerResult.Errors));
    
    int value = 0;

    if(command.VoteValue == 1) {
      answerResult.Value.Upvotes += 1;
      value = 10;
    }
    else {
      answerResult.Value.Downvotes += 1; 
      value = -5;
    }
    
    var updateResult = await _answerRepository.UpdateAsync (answerResult.Value, cancellationToken);
    
    if (!updateResult.IsSuccess)
      return Result.Error (new ErrorList(updateResult.Errors));
    
    await _messageBroker.PublishAsync(new AnswerVoted(answerResult.Value.UserId, value), cancellationToken: cancellationToken);
    await _answerRepository.SaveAsync(cancellationToken);
    
    return Result.Success ();
  }

}