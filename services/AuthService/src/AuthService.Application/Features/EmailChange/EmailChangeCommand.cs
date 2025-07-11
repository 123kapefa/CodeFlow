using AuthService.Application.Requests;

using Contracts.Commands;

namespace AuthService.Application.Features.RequestEmailChange;

public record EmailChangeCommand(Guid UserId, EmailChangeRequest Request) : ICommand;