using Ardalis.Result;

using QuestionService.Domain.Entities;
using QuestionService.Domain.Repositories;

using Abstractions.Commands;

namespace QuestionService.Application.Features.UpdateQuestionView;

public class UpdateQuestionViewHandler : ICommandHandler<UpdateQuestionViewCommand> {

  private readonly IQuestionServiceRepository _questionServiceRepository;

  public UpdateQuestionViewHandler (IQuestionServiceRepository questionServiceRepository) {
    _questionServiceRepository = questionServiceRepository;
  }

  public async Task<Result> Handle (UpdateQuestionViewCommand command, CancellationToken cancellationToken) {
    if (command.QuestionId == Guid.Empty)
      return Result.Error ("ID вопроса не может быть пустым");

    Result<Question> questionResult =
      await _questionServiceRepository.GetQuestionShortAsync (command.QuestionId, cancellationToken);

    if (!questionResult.IsSuccess)
      return Result.Error (new ErrorList (questionResult.Errors));

    questionResult.Value.ViewsCount += 1;

    Result updateResult =
      await _questionServiceRepository.UpdateQuestionAsync (questionResult.Value, cancellationToken);
      await _questionServiceRepository.SaveChangesAsync (cancellationToken);
    if (!updateResult.IsSuccess)
      return Result.Error (new ErrorList (updateResult.Errors));

    return Result.Success ();
  }

}