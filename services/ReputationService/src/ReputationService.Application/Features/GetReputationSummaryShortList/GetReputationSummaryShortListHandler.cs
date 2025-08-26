using Abstractions.Commands;

using Ardalis.Result;

using Contracts.DTOs.ReputationService;

using ReputationService.Application.Extensions;
using ReputationService.Domain.Repositories;

namespace ReputationService.Application.Features.GetReputationSummaryShortList;

public class
  GetReputationSummaryShortListHandler : ICommandHandler<PagedResult<IReadOnlyList<ReputationShortDto>>,
  GetReputationSummaryShortListCommand> {

  private readonly IReputationRepository _repository;

  public GetReputationSummaryShortListHandler (IReputationRepository repository) {
    _repository = repository;
  }

  public async Task<Result<PagedResult<IReadOnlyList<ReputationShortDto>>>> Handle (
    GetReputationSummaryShortListCommand command,
    CancellationToken cancellationToken) {
    var reputationsResult =
      await _repository.GetReputationShortList (command.UserId, command.PageParams, cancellationToken);

    if (!reputationsResult.IsSuccess) {
      return Result<PagedResult<IReadOnlyList<ReputationShortDto>>>.Error (new ErrorList(reputationsResult.Errors));;
    }
    
    var reputations = reputationsResult.Value.Items.ToReputationShortDtoList();
    
    return Result.Success (new PagedResult<IReadOnlyList<ReputationShortDto>> (reputationsResult.Value.Page, reputations));
  }

}