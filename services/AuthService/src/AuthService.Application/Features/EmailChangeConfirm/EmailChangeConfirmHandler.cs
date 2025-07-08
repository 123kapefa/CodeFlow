using Ardalis.Result;

using AuthService.Domain.Entities;

using Contracts.Commands;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.EmailChangeConfirm;

public class EmailChangeConfirmHandler : ICommandHandler<EmailChangeConfirmCommand> {

  private readonly UserManager<UserData> _userManager;
  private readonly ILogger<EmailChangeConfirmHandler> _logger;

  public EmailChangeConfirmHandler (UserManager<UserData> userManager, ILogger<EmailChangeConfirmHandler> logger) {
    _userManager = userManager;
    _logger = logger;
  }

  public async Task<Result> Handle (EmailChangeConfirmCommand command, CancellationToken ct) {
    var user = await _userManager.FindByIdAsync (command.UserId.ToString ());
    if (user == null)
      return Result.NotFound ($"Пользователь {command.UserId} не найден");

    var result = await _userManager.ChangeEmailAsync (user, command.Request.NewEmail, command.Request.Token);
    if (!result.Succeeded) {
      var errors = string.Join ("; ", result.Errors.Select (e => e.Description));
      _logger.LogWarning ("Подтверждение смены почты не удалось: {Errors}", errors);
      return Result.Error (errors);
    }

    _logger.LogInformation ("Почта для {UserId} изменёна на {NewEmail}", command.UserId
      , command.Request.NewEmail);
    return Result.Success ();
  }

}