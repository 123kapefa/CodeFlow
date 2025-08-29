using Abstractions.Commands;

using Contracts.Requests.AuthService;

namespace AuthService.Application.Features.EditUser;

public record EditUserCommand(Guid UserId, EditUserDataRequest Request) : ICommand;