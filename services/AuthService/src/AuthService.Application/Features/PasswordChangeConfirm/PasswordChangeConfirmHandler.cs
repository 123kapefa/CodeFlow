using Abstractions.Commands;

using Ardalis.Result;

using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;

using Microsoft.AspNetCore.Identity;

namespace AuthService.Application.Features.PasswordChangeConfirm;

public class PasswordChangeConfirmHandler : ICommandHandler<PasswordChangeConfirmCommand> {

  private readonly UserManager<UserData> _userManager;
  private readonly IPasswordChangeRepository _passwordChangeRepository;
  
  public PasswordChangeConfirmHandler (UserManager<UserData> userManager, IPasswordChangeRepository passwordChangeRepository) {
    _userManager = userManager;
    _passwordChangeRepository = passwordChangeRepository;
  }

  public async Task<Result> Handle (PasswordChangeConfirmCommand command, CancellationToken cancellationToken) {
    var user = await _userManager.FindByEmailAsync (command.Email);
    if (user == null) return Result.NotFound("Пользователь не найден.");
    
    var token = Uri.UnescapeDataString (command.Token);
    
    var newPassword = await _passwordChangeRepository.GetPasswordByTokenAsync(token);
    if (newPassword is null) return Result.Error("Неверный пароль или устаревший токен.");
    
    var result = await _userManager.ResetPasswordAsync (user, token, newPassword);
    
    if (!result.Succeeded) return Result.Error (new ErrorList(result.Errors.Select (e => e.Description)));

    await _passwordChangeRepository.RemoveAsync (token);
    
    return Result.Success ();
  }

}