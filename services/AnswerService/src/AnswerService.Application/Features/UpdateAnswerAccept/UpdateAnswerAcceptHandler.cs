using Abstractions.Commands;

using AnswerService.Domain.Repositories;

using Ardalis.Result;

namespace AnswerService.Application.Features.UpdateAnswerAccept;

public class UpdateAnswerAcceptHandler : ICommandHandler<UpdateAnswerAcceptCommand> {

  private readonly IAnswerRepository _answerRepository;

  public UpdateAnswerAcceptHandler (IAnswerRepository answerRepository) {
    _answerRepository = answerRepository;
  }

  public async Task<Result> Handle (UpdateAnswerAcceptCommand command, CancellationToken cancellationToken) {
    var answers = await _answerRepository.GetByQuestionIdAsync (command.QuestionId, cancellationToken);

    if (!answers.Value.Any (answer => answer.Id == command.Id)) {
      return Result.Error ("Ответ не найден.");
    }

    foreach (var answer in answers.Value) {
      if (answer.Id == command.Id) {
        answer.IsAccepted = true;
      } else {
        answer.IsAccepted = false;
      }
    }

    if (!answers.IsSuccess)
      return Result.Error (new ErrorList (answers.Errors));

    await _answerRepository.SaveAsync (cancellationToken);

    return Result.Success ();
  }

}