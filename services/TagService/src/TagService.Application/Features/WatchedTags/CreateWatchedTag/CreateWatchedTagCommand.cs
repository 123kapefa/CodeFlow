using Contracts.Commands;

namespace TagService.Application.Features.WatchedTags.CreateWatchedTag;

public record CreateWatchedTagCommand(Guid UserId, int TagId ) : ICommand;
