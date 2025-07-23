using Abstractions.Commands;

namespace TagService.Application.Features.WatchedTags.DeleteWatchedTag;

public record DeleteWatchedTagCommand(int TagId, Guid UserId) : ICommand;
