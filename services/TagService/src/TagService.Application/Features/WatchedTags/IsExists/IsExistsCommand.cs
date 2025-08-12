
using Abstractions.Commands;

namespace TagService.Application.Features.WatchedTags.IsExists;

public record IsExistsCommand( Guid UserId, int TagId ) : ICommand;
