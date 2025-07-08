using AuthService.Application.Requests;

using Contracts.Commands;

namespace AuthService.Application.Features.EmailChangeConfirm;

public record EmailChangeConfirmCommand(Guid UserId, EmailChangeConfirmRequest Request) : ICommand;