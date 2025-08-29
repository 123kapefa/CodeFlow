using Abstractions.Commands;
using Contracts.Requests.AuthService;

namespace AuthService.Application.Features.EmailChangeConfirm;

public record EmailChangeConfirmCommand(Guid UserId, EmailChangeConfirmRequest Request) : ICommand;