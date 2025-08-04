using Abstractions.Commands;

using Contracts.Requests.AuthService;

namespace AuthService.Application.Features.PasswordChange;

public record PasswordChangeCommand(
  Guid UserId,
  PasswordChangeRequest Request
  ) : ICommand;