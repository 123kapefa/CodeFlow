

using Contracts.Commands;

namespace AuthService.Application.Features.PasswordReset;

public record RequestPasswordResetCommand(string Email) : ICommand;