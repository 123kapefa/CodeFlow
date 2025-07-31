using Abstractions.Commands;

using AnswerService.Domain.Entities;
using AnswerService.Domain.Repositories;

using Ardalis.Result;

namespace AnswerService.Application.Features.DeleteAnswersByUserId;

public class DeleteAnswersByUserIdHandler : ICommandHandler<DeleteAnswersByUserIdCommand> {

  private readonly IAnswerRepository _answerRepository;
  
  public DeleteAnswersByUserIdHandler (IAnswerRepository answerRepository) {
    _answerRepository = answerRepository;
  }

  public async Task<Result> Handle (DeleteAnswersByUserIdCommand command, CancellationToken cancellationToken) {
    var answers = await _answerRepository.GetByUserIdAsync (command.UserId, cancellationToken);
    
    if (!answers.Value.Any())
      return Result.NotFound("Ответы пользователя не существуют.");
    
    await _answerRepository.DeleteAsync(answers.Value, cancellationToken);
    await _answerRepository.SaveAsync (cancellationToken);
    
    return Result.Success ();
  }

}