using Ardalis.Result;

using AuthService.Application.Abstractions;
using AuthService.Application.Response;
using AuthService.Domain.Repositories;

using Contracts.Commands;

using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.PasswordReset;

public class PasswordResetHandler : ICommandHandler<PasswordResetResponse, PasswordResetCommand> {

  private readonly IUserDataRepository _userDataRepository;
  private readonly ILogger<PasswordResetHandler> _logger;

  public PasswordResetHandler (IUserDataRepository userDataRepository, ILogger<PasswordResetHandler> logger) {
    _userDataRepository = userDataRepository;
    _logger = logger;
  }

  public async Task<Result<PasswordResetResponse>> Handle (PasswordResetCommand command, CancellationToken cancellationToken) {
    var user = await _userDataRepository.GetByEmailAsync(command.Email);
    if (!user.IsSuccess) return Result.Error("Такой почты не существует.");

    // TODO добавить отправку на почту
    var token = await _userDataRepository.GeneratePasswordResetTokenAsync(user.Value);
    //await _email.SendAsync(command.Email, "Reset password", token);
    return Result.Success(new PasswordResetResponse(true));
  }

} 