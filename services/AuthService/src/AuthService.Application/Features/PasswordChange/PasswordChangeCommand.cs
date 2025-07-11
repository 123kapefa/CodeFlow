

using AuthService.Application.Requests;

using Contracts.Commands;

namespace AuthService.Application.Features.PasswordChange;

public record PasswordChangeCommand(
  Guid UserId,
  PasswordChangeRequest Request
  ) : ICommand;