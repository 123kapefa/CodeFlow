using AnswerService.Application.Extensions;
using AnswerService.Application.Responses;
using AnswerService.Domain.Repositories;

using Ardalis.Result;

using Contracts.Commands;

namespace AnswerService.Application.Features.GetAnswersByQuestionId;

public class GetAnswersByQuestionIdHandler
  : ICommandHandler <GetAnswersResponse, GetAnswersByQuestionIdCommand> {

  private readonly IAnswerRepository _answerRepository;
  
  public GetAnswersByQuestionIdHandler (IAnswerRepository answerRepository) {
    _answerRepository = answerRepository;
  }

  public async Task<Result<GetAnswersResponse>> Handle (
    GetAnswersByQuestionIdCommand command, 
    CancellationToken cancellationToken) {
    var answers = await _answerRepository
     .GetByQuestionIdAsync (command.QuestionId, cancellationToken);

    if (!answers.IsSuccess) {
      return Result<GetAnswersResponse>.Error (new ErrorList (answers.Errors));
    }
    
    return Result<GetAnswersResponse>.Success (new GetAnswersResponse(answers.Value.ToDto()));
  }
}