using AnswerService.Domain.Repositories;

using Ardalis.Result;

using Abstractions.Commands;

namespace AnswerService.Application.Features.AcceptAnswer;

public class AcceptAnswerHandler : ICommandHandler<AcceptAnswerCommand> {

  private readonly IAnswerRepository _answerRepository;
  
  public AcceptAnswerHandler (IAnswerRepository answerRepository) {
    _answerRepository = answerRepository;
  }

  public async Task<Result> Handle (AcceptAnswerCommand command, CancellationToken cancellationToken) {
    // var answer = await _answerRepository.GetByIdAsync(command.Id, cancellationToken);
    //
    // if (!answer.IsSuccess)
    //   return Result.Error (new ErrorList(answer.Errors));
    
    var answers = await _answerRepository
     .GetByQuestionIdAsync (command.QuestionId, cancellationToken);

    if (!answers.Value.Any (answer => answer.Id == command.Id)) {
      return Result.Error ("Ответ не найден.");
    }
    
    foreach (var answer in answers.Value) {
      if (answer.Id == command.Id) {
        answer.IsAccepted = true;
      }
      else {
        answer.IsAccepted = false;
      }
    }
    
    if (!answers.IsSuccess)
      return Result.Error (new ErrorList(answers.Errors));
    
    // var acceptedAnswerResult = await _answerRepository
    //  .AcceptAsync (answers.Value, answer.Value.Id, cancellationToken);
    //
    // if (!acceptedAnswerResult.IsSuccess)
    //   return Result.Error (new ErrorList(acceptedAnswerResult.Errors));
    
    await _answerRepository.SaveAsync(cancellationToken);
    
    return Result.Success();
  }

}