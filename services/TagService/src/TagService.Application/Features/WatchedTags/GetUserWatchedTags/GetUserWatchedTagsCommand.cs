using Contracts.Commands;

namespace TagService.Application.Features.WatchedTags.GetUserWatchedTags;

public record GetUserWatchedTagsCommand(Guid UserId) : ICommand;
