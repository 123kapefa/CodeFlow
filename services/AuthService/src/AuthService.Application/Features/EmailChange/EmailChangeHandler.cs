using System.Net;
using System.Text.Encodings.Web;

using Ardalis.Result;
using Ardalis.Result.FluentValidation;

using AuthService.Application.Abstractions;
using AuthService.Domain.Entities;

using Contracts.Commands;

using FluentValidation;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.RequestEmailChange;

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
    if (!validationResult.IsValid)
      return Result.Invalid (validationResult.AsErrors ());
    
    var user = await _userManager.FindByIdAsync (command.UserId.ToString ());
    if (user == null)
      return Result.NotFound ($"Пользователь {command.UserId} не найден");

    if (user.Email != command.Request.OldEmail)
      return Result.Error("Неверная указана старая почта.");

    var token = await _userManager.GenerateChangeEmailTokenAsync (user, command.Request.NewEmail);
    
    var frontend = _configuration["Frontend:Url"]!.TrimEnd ('/');
    var url = $"{frontend}/confirm-email-change?" + $"userId={WebUtility.UrlEncode (command.UserId.ToString ())}" +
      $"&newEmail={WebUtility.UrlEncode (command.Request.NewEmail)}" + $"&token={WebUtility.UrlEncode (token)}";

    var body = $"Чтобы подтвердить новую почту <b>{command.Request.NewEmail}</b>, перейдите по ссылке:<br>" +
      $"<a href=\"{HtmlEncoder.Default.Encode (url)}\">Подтвердить новую почту</a>";
    
    await _emailSender.SendEmailAsync (command.Request.NewEmail, "Подтвердите смену e-mail", body);

    _logger.LogInformation ("Запрошена смена почты для {UserId} на {NewEmail}", command.UserId, command.Request.NewEmail);
    return Result.Success ();
  }

}