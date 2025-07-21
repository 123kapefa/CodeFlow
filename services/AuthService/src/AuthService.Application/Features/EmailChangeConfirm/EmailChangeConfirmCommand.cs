using Abstractions.Commands;
using Contracts.AuthService.Requests;

namespace AuthService.Application.Features.EmailChangeConfirm;

public record EmailChangeConfirmCommand(Guid UserId, EmailChangeConfirmRequest Request) : ICommand;