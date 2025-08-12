using Abstractions.Commands;

namespace UserService.Application.Features.GetUsersByIds;

public record GetUsersByIdsCommand (IEnumerable<Guid> UserIds) : ICommand;