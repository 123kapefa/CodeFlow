using Abstractions.Commands;
using Contracts.AuthService.Requests;

namespace AuthService.Application.Features.EditUser;

public record EditUserCommand(Guid UserId, EditUserDataRequest Request) : ICommand;