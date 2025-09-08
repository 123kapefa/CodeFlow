using System.Net;

using Abstractions.Commands;

using Ardalis.Result;

using AuthService.Application.Abstractions;
using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.PasswordChange;

public class PasswordChangeHandler : ICommandHandler<PasswordChangeCommand> {
  
  private readonly UserManager<UserData> _userManager;
  private readonly IPasswordChangeRepository _passwordChangeRepository;
  private readonly IEmailSender _emailSender;
  private readonly ILogger<PasswordChangeHandler> _logger;

  public PasswordChangeHandler (
    UserManager<UserData> userManager
    , IPasswordChangeRepository passwordChangeRepository
    , IEmailSender emailSender
    , ILogger<PasswordChangeHandler> logger) { 
    _userManager = userManager;
    _passwordChangeRepository = passwordChangeRepository;
    _emailSender = emailSender;
    _logger = logger;
  }

  public async Task<Result> Handle (
    PasswordChangeCommand command, 
    CancellationToken cancellationToken) {
    var user = await _userManager.FindByIdAsync(command.UserId.ToString());
    if (user is null) return Result.NotFound("Пользователь не найден");

    if (!await _userManager.CheckPasswordAsync(user, command.Request.OldPassword))
      return Result.Invalid(new ValidationError(
        "password.not.valid", 
        "Старый пароль неверен", 
        "422",
        ValidationSeverity.Info));

    if (command.Request.NewPassword != command.Request.ConfirmNewPassword)
      return Result.Invalid(new ValidationError(
        "password.not.valid", 
        "Новые пароли не совпадают", 
        "422", 
        ValidationSeverity.Info));

    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        Console.WriteLine("****");
        Console.WriteLine("****");
        Console.WriteLine($" PasswordChangeHandler token => {token}");
        Console.WriteLine($"PasswordChangeHandler encodedToken => {Uri.EscapeDataString(token)}");
        Console.WriteLine(" ****");
        Console.WriteLine("****");

        await _passwordChangeRepository.SaveAsync(user.Id, command.Request.NewPassword, token);

    var encodedToken = Uri.EscapeDataString(token);
        var url = $"{Environment.GetEnvironmentVariable("URI")}/password-change-confirm?email={user.Email}&token={encodedToken}";
        var html = $"<p>Вы запросили смену пароля. Подтвердите:</p><p><a href='{url}'>Подтвердить смену</a></p>";

    await _emailSender.SendEmailAsync(user.Email!, "Подтверждение смены пароля", html);

    return Result.Success();
  }
}