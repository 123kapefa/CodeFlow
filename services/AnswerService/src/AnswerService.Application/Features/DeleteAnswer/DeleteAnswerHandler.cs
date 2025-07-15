using AnswerService.Domain.Repositories;

using Ardalis.Result;

using Contracts.Commands;

namespace AnswerService.Application.Features.DeleteAnswer;

public class DeleteAnswerHandler : ICommandHandler<DeleteAnswerCommand> {

  private readonly IAnswerRepository _answerRepository;
  
  public DeleteAnswerHandler (IAnswerRepository answerRepository) {
    _answerRepository = answerRepository;
  }

  public async Task<Result> Handle (DeleteAnswerCommand command, CancellationToken cancellationToken) {
    
    var answer = await _answerRepository.GetByIdAsync (command.Id, cancellationToken);

    if (!answer.IsSuccess) {
      return Result.Error (new ErrorList (answer.Errors));
    }
    
    await _answerRepository.DeleteAsync (answer, cancellationToken);
    return Result.Success ();
  }

}