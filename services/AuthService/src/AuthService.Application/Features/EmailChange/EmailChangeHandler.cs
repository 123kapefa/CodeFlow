using System.Net;
using System.Text.Encodings.Web;

using Abstractions.Commands;

using Ardalis.Result;
using Ardalis.Result.FluentValidation;

using AuthService.Application.Abstractions;
using AuthService.Domain.Entities;

using FluentValidation;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.EmailChange;

public class EmailChangeHandler : ICommandHandler<EmailChangeCommand> {

  private readonly UserManager<UserData> _userManager;
  private readonly IEmailSender _emailSender;
  private readonly IValidator<EmailChangeCommand> _validator;
  private readonly IConfiguration _configuration;
  private readonly ILogger<EmailChangeHandler> _logger;

  public EmailChangeHandler (
    UserManager<UserData> userManager
    , IEmailSender emailSender
    , IValidator<EmailChangeCommand> validator
    , IConfiguration configuration
    , ILogger<EmailChangeHandler> logger) {
    _userManager = userManager;
    _emailSender = emailSender;
    _validator = validator;
    _configuration = configuration;
    _logger = logger;
  }

  public async Task<Result> Handle (EmailChangeCommand command, CancellationToken ct) {
    var validationResult = await _validator.ValidateAsync(command, ct);

        Console.WriteLine("***");
        Console.WriteLine("***");
        Console.WriteLine($"EmailChangeHandler OldEmail => {command.Request.OldEmail}");
        Console.WriteLine($"EmailChangeHandler NewEmail => {command.Request.NewEmail}");
        Console.WriteLine("***");
        Console.WriteLine("***");

        if (!validationResult.IsValid)
      return Result.Invalid (validationResult.AsErrors ());
    
    var user = await _userManager.FindByIdAsync (command.UserId.ToString ());
    if (user == null)
      return Result.NotFound ($"Пользователь {command.UserId} не найден");

    if (user.Email != command.Request.OldEmail)
      return Result.Error("Неверная указана старая почта.");

    var token = await _userManager.GenerateChangeEmailTokenAsync (user, command.Request.NewEmail);

        var encodedToken = Uri.EscapeDataString(token);

      
    var url = $"http://localhost:3000/email-change-confirm?email={command.Request.NewEmail}&token={encodedToken}";

    var body = $"Чтобы подтвердить новую почту перейдите по ссылке:<br>" +
      $"<a href='{url}' >Подтвердить новую почту</a>";

    
    await _emailSender.SendEmailAsync (command.Request.NewEmail, "Подтвердите смену e-mail", body);

    _logger.LogInformation ("Запрошена смена почты для {UserId} на {NewEmail}", command.UserId, command.Request.NewEmail);
    return Result.Success ();
  }

}