using AnswerService.Domain.Repositories;

using Ardalis.Result;

using Abstractions.Commands;

using Contracts.Publishers.AnswerService;

using Messaging.Broker;

namespace AnswerService.Application.Features.DeleteAnswer;

public class DeleteAnswerHandler : ICommandHandler<DeleteAnswerCommand> {

  private readonly IAnswerRepository _answerRepository;
  private readonly IMessageBroker _messageBroker;
  
  public DeleteAnswerHandler (IAnswerRepository answerRepository, IMessageBroker messageBroker) {
    _answerRepository = answerRepository;
    _messageBroker = messageBroker;
  }

  public async Task<Result> Handle (DeleteAnswerCommand command, CancellationToken cancellationToken) {
    
    var answer = await _answerRepository.GetByIdAsync (command.Id, cancellationToken);

    if (!answer.IsSuccess) {
      return Result.Error (new ErrorList (answer.Errors));
    }
    
    await _answerRepository.DeleteAsync (answer, cancellationToken);
    await _messageBroker.PublishAsync (
      new AnswerDeleted (
        answer.Value.UserId, 
        command.Request.QuestionId,
        answer.Value.Id,
        command.Request.TagIds), 
      cancellationToken);
    await _answerRepository.SaveAsync (cancellationToken);
    
    return Result.Success ();
  }

}