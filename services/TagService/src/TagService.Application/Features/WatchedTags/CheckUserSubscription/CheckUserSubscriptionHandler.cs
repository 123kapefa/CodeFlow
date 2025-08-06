using Abstractions.Commands;

using Ardalis.Result;

using Contracts.Responses.TagService;

using TagService.Domain.Repositories;

namespace TagService.Application.Features.WatchedTags.CheckUserSubscription;

public class
  CheckUserSubscriptionHandler : ICommandHandler<CheckUserSubcriptionResponse, CheckUserSubscriptionCommand> {

  private readonly IWatchedTagRepository _watchedTagRepository;

  public CheckUserSubscriptionHandler (IWatchedTagRepository watchedTagRepository) {
    _watchedTagRepository = watchedTagRepository;
  }

  public async Task<Result<CheckUserSubcriptionResponse>> Handle (
    CheckUserSubscriptionCommand command
    , CancellationToken cancellationToken) {
    var result = await _watchedTagRepository.ExistsAsync (command.UserId, command.TagId, cancellationToken);

    return result
      ? Result.Success (new CheckUserSubcriptionResponse (true))
      : Result.Success (new CheckUserSubcriptionResponse (false));
  }

}