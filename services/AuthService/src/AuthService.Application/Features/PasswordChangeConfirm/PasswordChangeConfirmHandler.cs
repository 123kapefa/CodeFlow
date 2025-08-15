using Abstractions.Commands;

using Ardalis.Result;

using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;

using Microsoft.AspNetCore.Identity;
using System.Net;

namespace AuthService.Application.Features.PasswordChangeConfirm;

public class PasswordChangeConfirmHandler : ICommandHandler<PasswordChangeConfirmCommand> {

  private readonly UserManager<UserData> _userManager;
  private readonly IPasswordChangeRepository _passwordChangeRepository;
  
  public PasswordChangeConfirmHandler (UserManager<UserData> userManager, IPasswordChangeRepository passwordChangeRepository) {
    _userManager = userManager;
    _passwordChangeRepository = passwordChangeRepository;
  }

  public async Task<Result> Handle (PasswordChangeConfirmCommand command, CancellationToken cancellationToken) {
        var user = await _userManager.FindByEmailAsync(command.Email);
        if(user is null) return Result.NotFound("Пользователь не найден.");

        // ── 1) НОРМАЛИЗАЦИЯ ТОКЕНА ─────────────────────────────────────────────
        // Если вы уже отправляете токен как Uri.EscapeDataString(rawToken):
        //   нормализуем так:
        var normalized = WebUtility.UrlDecode(command.Token ?? string.Empty).Replace(' ', '+');
        var rawToken = normalized;

        Console.WriteLine("****");
        Console.WriteLine("****");
        Console.WriteLine($"token => {command.Token}");
        Console.WriteLine($"normalized => {normalized}");
            Console.WriteLine($"rawToken => {rawToken}");
        Console.WriteLine(" ****");
        Console.WriteLine("****");

        // Если перейдёте на Base64Url (рекомендую) — замените две строки выше на:
        // var rawToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(normalized));

        // ── 2) Достаём ожидающий пароль по ТОМУ ЖЕ rawToken ───────────────────
        var newPassword = await _passwordChangeRepository.GetPasswordByTokenAsync(command.Token);
        if(newPassword is null)
            return Result.Error("Неверный или просроченный токен.");

        // ── 3) РЕТРАЙ при ConcurrencyFailure (до 2 попыток) ───────────────────
        for(int attempt = 1; attempt <= 2; attempt++) {
            var reset = await _userManager.ResetPasswordAsync(user, command.Token, newPassword);
            if(reset.Succeeded) {

                Console.WriteLine("****");
                Console.WriteLine("****");
                Console.WriteLine($"Succeeded ");              
                Console.WriteLine(" ****");
               

                try { await _passwordChangeRepository.RemoveAsync(rawToken); } catch { /* no-op */ }
                return Result.Success();
            }

            // если именно конфликт конкурентности — перезагружаем пользователя и повторяем
            if(reset.Errors.Any(e =>
                    string.Equals(e.Code, nameof(IdentityErrorDescriber.ConcurrencyFailure),
                                  StringComparison.OrdinalIgnoreCase))) {

                Console.WriteLine("****");
                Console.WriteLine("****");
                Console.WriteLine($"Errors ");
                Console.WriteLine(" ****");

                // свежая копия пользователя
                user = await _userManager.FindByIdAsync(user.Id.ToString());
                continue;
            }

            // прочие ошибки — наружу
            return Result.Error(new ErrorList(reset.Errors.Select(e => e.Description)));
        }

        return Result.Error("Конфликт конкурентности. Повторите попытку.");
    }

}