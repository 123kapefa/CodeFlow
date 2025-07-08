using Contracts.Commands;

namespace AuthService.Application.Features.RemoveUser;

public record RemoveUserCommand (Guid UserId) : ICommand;