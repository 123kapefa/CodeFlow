using Abstractions.Commands;

using AnswerService.Domain.Repositories;

using Ardalis.Result;

using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace AnswerService.Application.Features.UpdateAnswerAccept;

public class UpdateAnswerAcceptHandler : ICommandHandler<UpdateAnswerAcceptCommand> {

  private readonly IAnswerRepository _answerRepository;
    private readonly ILogger<UpdateAnswerAcceptHandler> _logger;

    public UpdateAnswerAcceptHandler( IAnswerRepository answerRepository, ILogger<UpdateAnswerAcceptHandler> logger ) {
        _answerRepository = answerRepository;
        _logger = logger;
    }

    public async Task<Result> Handle (UpdateAnswerAcceptCommand command, CancellationToken cancellationToken) {

    var answers = await _answerRepository.GetByQuestionIdAsync (command.QuestionId, cancellationToken);

    if (!answers.Value.Any (answer => answer.Id == command.Id)) {
      return Result.Error ("Ответ не найден.");
    }

        _logger.LogInformation($"Получение списка ответов по {JsonSerializer.Serialize( answers.Value)}." );

        foreach (var answer in answers.Value) {
      if (answer.Id == command.Id) {
        answer.IsAccepted = true;
      } else {
        answer.IsAccepted = false;
      }
    }

    if (!answers.IsSuccess)
      return Result.Error (new ErrorList (answers.Errors));


        await _answerRepository.UpdateAsync(answers.Value, cancellationToken);



        await _answerRepository.SaveAsync (cancellationToken);

        _logger.LogInformation($"СОХРАНЕНИЕ  {JsonSerializer.Serialize(answers.Value)}.");

        return Result.Success ();
  }

}