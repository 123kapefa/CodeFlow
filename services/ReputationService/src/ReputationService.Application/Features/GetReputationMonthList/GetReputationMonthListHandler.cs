using Abstractions.Commands;

using Ardalis.Result;

using Contracts.DTOs.ReputationService;

using ReputationService.Application.Extensions;
using ReputationService.Domain.Repositories;

namespace ReputationService.Application.Features.GetReputationMonthList;

public class GetReputationMonthListHandler : ICommandHandler<IReadOnlyList<ReputationShortDto>, GetReputationMonthListCommand> {

  private readonly IReputationRepository _repository;
  
  public GetReputationMonthListHandler (IReputationRepository repository) {
    _repository = repository;
  }

  public async Task<Result<IReadOnlyList<ReputationShortDto>>> Handle (GetReputationMonthListCommand command, CancellationToken cancellationToken) {
    var result = await _repository.GetMonthReputationAsync(command.UserId, cancellationToken);

    if (!result.IsSuccess)
      return Result<IReadOnlyList<ReputationShortDto>>.Error (new ErrorList(result.Errors));
    
    Console.WriteLine($"====== {result.Value.Count()} ======");
    
    var reputations = result.Value.ToReputationShortDtoList ();
    
    Console.WriteLine($"====== {reputations.Count()} ======");
    
    return Result<IReadOnlyList<ReputationShortDto>>.Success (reputations);
  }

}