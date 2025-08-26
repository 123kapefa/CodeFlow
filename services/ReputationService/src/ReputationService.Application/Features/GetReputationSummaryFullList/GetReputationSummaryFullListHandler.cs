using Abstractions.Commands;

using Ardalis.Result;

using Contracts.DTOs.ReputationService;

using ReputationService.Application.Extensions;
using ReputationService.Application.Features.GetReputationSummaryShortList;
using ReputationService.Domain.Repositories;

namespace ReputationService.Application.Features.GetReputationSummaryFullList;

public class
  GetReputationSummaryFullListHandler : ICommandHandler<PagedResult<IReadOnlyList<ReputationGroupedByDayDto>>, GetReputationSummaryFullListCommand> {

  private readonly IReputationRepository _repository;

  public GetReputationSummaryFullListHandler (IReputationRepository repository) {
    _repository = repository;
  }

  public async Task<Result<PagedResult<IReadOnlyList<ReputationGroupedByDayDto>>>> Handle (
    GetReputationSummaryFullListCommand command,
    CancellationToken cancellationToken) {
    var reputationsResult =
      await _repository.GetReputationFullList (command.UserId, command.PageParams, cancellationToken);

    if (!reputationsResult.IsSuccess) {
      return Result<PagedResult<IReadOnlyList<ReputationGroupedByDayDto>>>.Error (new ErrorList(reputationsResult.Errors));;
    }
    
    var reputations = reputationsResult.Value.Groups.ToGroupedDtoList ();
    
    return Result.Success (new PagedResult<IReadOnlyList<ReputationGroupedByDayDto>> (reputationsResult.Value.Page, reputations));
  }

}