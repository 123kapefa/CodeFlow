using Abstractions.Commands;

using Ardalis.Result;

using Contracts.DTOs.TagService;

using TagService.Application.Extensions;
using TagService.Domain.Repositories;

namespace TagService.Application.Features.WatchedTags.GetWatchedTagsByUserId;

public class GetWatchedTagsByUserIdHandler : ICommandHandler<IEnumerable<WatchedTagDTO>, GetWatchedTagsByUserIdCommand> {

  private readonly IWatchedTagRepository _watchedTagRepository;
  
  public GetWatchedTagsByUserIdHandler (IWatchedTagRepository watchedTagRepository) {
    _watchedTagRepository = watchedTagRepository;
  }

  public async Task<Result<IEnumerable<WatchedTagDTO>>> Handle (GetWatchedTagsByUserIdCommand command, CancellationToken cancellationToken) {
    var result = await _watchedTagRepository.GetUserWatchedTagsListAsync (command.UserId, cancellationToken);

    return Result<IEnumerable<WatchedTagDTO>>.Success (result.ToWatchedTagsDto ());

  }

}