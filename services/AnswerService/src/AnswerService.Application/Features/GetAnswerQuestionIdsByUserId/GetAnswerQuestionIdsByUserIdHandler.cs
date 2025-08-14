using Abstractions.Commands;

using AnswerService.Domain.Repositories;

using Ardalis.Result;

namespace AnswerService.Application.Features.GetAnswerQuestionIdsByUserId;

public class GetAnswerQuestionIdsByUserIdHandler : ICommandHandler<IEnumerable<Guid>, GetAnswerQuestionIdsByUserIdCommand> {

  private readonly IAnswerRepository _answerRepository;
  
  public GetAnswerQuestionIdsByUserIdHandler (IAnswerRepository answerRepository) {
    _answerRepository = answerRepository;
  }

  public async Task<Result<IEnumerable<Guid>>> Handle (GetAnswerQuestionIdsByUserIdCommand command, CancellationToken cancellationToken) {
    var result = await _answerRepository.GetQuestionIdsByUserId (command.UserId, command.PageParams, command.SortParams, cancellationToken);
    
    if (!result.IsSuccess)
      return Result.Error (new ErrorList (result.Errors));
    
    return Result.Success (result.Value);
  }

}