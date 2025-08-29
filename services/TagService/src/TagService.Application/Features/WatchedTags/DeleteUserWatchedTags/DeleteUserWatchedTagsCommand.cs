using Abstractions.Commands;

namespace TagService.Application.Features.WatchedTags.DeleteUserWatchedTags;

//TODO НЕ НУЖЕН !!!! !!! !!!
public record DeleteUserWatchedTagsCommand(Guid UserId) : ICommand;
