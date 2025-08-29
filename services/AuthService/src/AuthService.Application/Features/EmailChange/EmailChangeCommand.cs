using Abstractions.Commands;

using Contracts.Requests.AuthService;

namespace AuthService.Application.Features.EmailChange;

public record EmailChangeCommand(Guid UserId, EmailChangeRequest Request) : ICommand;