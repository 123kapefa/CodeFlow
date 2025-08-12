using AnswerService.Application.Extensions;
using AnswerService.Domain.Repositories;

using Ardalis.Result;

using Abstractions.Commands;

using Contracts.DTOs.AnswerService;
using Contracts.Responses.AnswerService;

namespace AnswerService.Application.Features.GetAnswersByQuestionId;

public class GetAnswersByQuestionIdHandler
  : ICommandHandler <IEnumerable<AnswerDto>, GetAnswersByQuestionIdCommand> {

  private readonly IAnswerRepository _answerRepository;
  
  public GetAnswersByQuestionIdHandler (IAnswerRepository answerRepository) {
    _answerRepository = answerRepository;
  }

  public async Task<Result<IEnumerable<AnswerDto>>> Handle (
    GetAnswersByQuestionIdCommand command, 
    CancellationToken cancellationToken) {
    var answers = await _answerRepository
     .GetByQuestionIdAsync (command.QuestionId, cancellationToken);

    if (!answers.IsSuccess) {
      return Result<IEnumerable<AnswerDto>>.Error (new ErrorList (answers.Errors));
    }
    
    return Result<IEnumerable<AnswerDto>>.Success (answers.Value.ToAnswersDto ());
  }
}