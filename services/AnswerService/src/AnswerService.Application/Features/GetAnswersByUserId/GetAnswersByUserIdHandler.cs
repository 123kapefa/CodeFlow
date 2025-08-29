using AnswerService.Application.Extensions;
using AnswerService.Domain.Repositories;

using Ardalis.Result;

using Abstractions.Commands;

using Contracts.DTOs.AnswerService;
using Contracts.Responses.AnswerService;

namespace AnswerService.Application.Features.GetAnswersByUserId;

public class GetAnswersByUserIdHandler : ICommandHandler<PagedResult<IEnumerable<AnswerDto>>, GetAnswersByUserIdCommand> {

  private readonly IAnswerRepository _answerRepository;
  
  public GetAnswersByUserIdHandler (IAnswerRepository answerRepository) {
    _answerRepository = answerRepository;
  }

  public async Task<Result<PagedResult<IEnumerable<AnswerDto>>>> Handle (GetAnswersByUserIdCommand command, CancellationToken cancellationToken) {
    
    var result = await _answerRepository.GetByUserIdAsync (command.UserId, command.PageParams, command.SortParams,  cancellationToken);
    
    if(!result.IsSuccess)
      return Result.Error(new ErrorList(result.Errors));
    
    IEnumerable<AnswerDto> answersDto = result.Value.items.ToAnswersDto ();
    
    return Result<PagedResult<IEnumerable<AnswerDto>>>
     .Success(new PagedResult<IEnumerable<AnswerDto>>(result.Value.pageInfo, answersDto));

  }
}