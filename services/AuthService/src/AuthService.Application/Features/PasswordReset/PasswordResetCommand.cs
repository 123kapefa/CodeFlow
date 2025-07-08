

using Contracts.Commands;

namespace AuthService.Application.Features.PasswordReset;

public record PasswordResetCommand(string Email) : ICommand;