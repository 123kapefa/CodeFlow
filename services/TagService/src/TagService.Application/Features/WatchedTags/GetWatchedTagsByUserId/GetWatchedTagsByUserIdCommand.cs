using Abstractions.Commands;

namespace TagService.Application.Features.WatchedTags.GetWatchedTagsByUserId;

public record GetWatchedTagsByUserIdCommand (Guid UserId) : ICommand;