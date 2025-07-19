using AuthService.Application.Requests;

using Contracts.Commands;

namespace AuthService.Application.Features.EditUser;

public record EditUserCommand(Guid UserId, EditUserDataRequest Request) : ICommand;