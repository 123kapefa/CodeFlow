using Abstractions.Commands;

using Contracts.AuthService.Requests;

namespace AuthService.Application.Features.EmailChange;

public record EmailChangeCommand(Guid UserId, EmailChangeRequest Request) : ICommand;