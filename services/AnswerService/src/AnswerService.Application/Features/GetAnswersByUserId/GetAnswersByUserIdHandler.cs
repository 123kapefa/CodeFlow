using AnswerService.Application.Extensions;
using AnswerService.Domain.Repositories;

using Ardalis.Result;

using Abstractions.Commands;

using Contracts.AnswerService.Responses;

namespace AnswerService.Application.Features.GetAnswersByUserId;

public class GetAnswersByUserIdHandler : ICommandHandler<GetAnswersResponse, GetAnswersByUserIdCommand> {

  private readonly IAnswerRepository _answerRepository;
  
  public GetAnswersByUserIdHandler (IAnswerRepository answerRepository) {
    _answerRepository = answerRepository;
  }

  public async Task<Result<GetAnswersResponse>> Handle (GetAnswersByUserIdCommand command, CancellationToken cancellationToken) {
    
    var answers = await _answerRepository.GetByUserIdAsync (command.UserId,  cancellationToken);
    
    return !answers.IsSuccess
      ? Result<GetAnswersResponse>.Error(new ErrorList(answers.Errors))
      : Result<GetAnswersResponse>.Success (new GetAnswersResponse(answers.Value.ToDto()));
  }
}