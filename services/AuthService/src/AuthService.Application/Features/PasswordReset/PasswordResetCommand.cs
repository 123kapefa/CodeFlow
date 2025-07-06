using AuthService.Application.Abstractions;

namespace AuthService.Application.Features.PasswordReset;

public record RequestPasswordResetCommand(string Email) : ICommand;