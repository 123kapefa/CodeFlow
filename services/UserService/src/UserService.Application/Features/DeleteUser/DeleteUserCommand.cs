using Abstractions.Commands;

namespace UserService.Application.Features.DeleteUser;

public record DeleteUserCommand (Guid UserId) : ICommand;