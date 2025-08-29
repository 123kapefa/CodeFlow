using Abstractions.Commands;
using Ardalis.Result;
using AuthService.Domain.Entities;

using Contracts.Responses.AuthService;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.EditUser;

public class EditUserHandler : ICommandHandler<EditUserDataResponse, EditUserCommand> {

  private readonly UserManager<UserData> _userManager;
  private readonly ILogger<EditUserHandler> _logger;

  public EditUserHandler (UserManager<UserData> userManager, ILogger<EditUserHandler> logger) {
    _userManager = userManager;
    _logger = logger;
  }

  public async Task<Result<EditUserDataResponse>> Handle (
    EditUserCommand command
    , CancellationToken cancellationToken) {
    _logger.LogInformation ("Редактирование IdentityUser {UserId}", command.UserId);

    var user = await _userManager.FindByIdAsync (command.UserId.ToString ());
    if (user == null) {
      _logger.LogWarning ("User {UserId} не найден", command.UserId);

      return Result.NotFound ($"Пользователь {command.UserId} не найден");
    }


    if (!string.IsNullOrWhiteSpace (command.Request.Username) && 
        !string.Equals (user.UserName, command.Request.Username
          , StringComparison.OrdinalIgnoreCase)) {
      var setNameResult = await _userManager.SetUserNameAsync (user, command.Request.Username);
      if (!setNameResult.Succeeded) {
        var errors = string.Join ("; ", setNameResult.Errors.Select (e => e.Description));
        _logger.LogError ("Ошибка установки UserName: {Errors}", errors);
        
        return Result.Error (errors);
      }
    }
    
    if (!string.IsNullOrWhiteSpace (command.Request.PhoneNumber) && !string.Equals (user.PhoneNumber
          , command.Request.PhoneNumber, StringComparison.OrdinalIgnoreCase)) {
      var setPhone = await _userManager.SetPhoneNumberAsync (user, command.Request.PhoneNumber);
      if (!setPhone.Succeeded) {
        var errors = string.Join ("; ", setPhone.Errors.Select (e => e.Description));
        _logger.LogError ("Ошибка установки PhoneNumber: {Errors}", errors);
        
        return Result.Error (errors);
      }
    }
    
    var update = await _userManager.UpdateAsync (user);
    if (!update.Succeeded) {
      var errors = string.Join ("; ", update.Errors.Select (e => e.Description));
      _logger.LogError ("Ошибка при обновлении пользователя: {Errors}", errors);
      return Result.Error (errors);
    }

    _logger.LogInformation ("IdentityUser {UserId} успешно обновлён", command.UserId);
    return Result.Success ();
  }

}