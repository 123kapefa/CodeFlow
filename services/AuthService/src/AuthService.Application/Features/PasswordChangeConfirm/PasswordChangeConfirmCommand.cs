using Abstractions.Commands;

namespace AuthService.Application.Features.PasswordChangeConfirm;

public record PasswordChangeConfirmCommand (string Email, string Token) : ICommand;