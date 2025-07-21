using Abstractions.Commands;

using Contracts.AuthService.Requests;

namespace AuthService.Application.Features.PasswordChange;

public record PasswordChangeCommand(
  Guid UserId,
  PasswordChangeRequest Request
  ) : ICommand;