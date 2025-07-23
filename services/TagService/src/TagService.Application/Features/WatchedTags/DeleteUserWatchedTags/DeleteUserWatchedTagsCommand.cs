using Abstractions.Commands;

namespace TagService.Application.Features.WatchedTags.DeleteUserWatchedTags;

public record DeleteUserWatchedTagsCommand(Guid UserId) : ICommand;
