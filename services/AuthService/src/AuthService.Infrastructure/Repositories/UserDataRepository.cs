using Ardalis.Result;

using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Repositories;

public class UserDataRepository : IUserDataRepository {

  private readonly IRefreshTokenRepository _refreshTokenRepository;
  private readonly UserManager<UserData> _userManager;
  private readonly AuthServiceDbContext _context;

  public UserDataRepository (UserManager<UserData> userManager, AuthServiceDbContext context
    , IRefreshTokenRepository refreshTokenRepository) {
    _userManager = userManager;
    _context = context;
    _refreshTokenRepository = refreshTokenRepository;
  }

  /// <summary> Получение пользователя по ID </summary>
  public async Task<Result<UserData>> GetByIdAsync (Guid userId) {
    var userData = await _userManager.FindByIdAsync (userId.ToString ());

    if (userData == null) {
      Result<UserData>.NotFound ($"Пользователь с таким ID: {userId} не найден.");
    }

    return Result<UserData>.Success (userData!);
  }

  /// <summary> Получение пользователя по Email </summary>
  public async Task<Result<UserData>> GetByEmailAsync (string email) {
    var userData = await _userManager.FindByEmailAsync (email);

    if (userData == null) {
      Result<UserData>.NotFound ($"Пользователь с такой почтой: {email} не найден.");
    }

    return Result<UserData>.Success (userData!);
  }

  /// <summary> Создание пользователя </summary>
  public async Task<Result<Guid>> CreateAsync (UserData user, string? password = null) {

        UserData? isExist = await _userManager.FindByEmailAsync(user.Email);

        if(isExist is not null && !EqualityComparer<object>.Default.Equals(
            await _userManager.GetUserIdAsync(isExist), await _userManager.GetUserIdAsync(user))) {
            return Result<Guid>.Conflict($"Email {user.Email} is already taken.");
        }

        IdentityResult result = password is null
      ? await _userManager.CreateAsync (user)
      : await _userManager.CreateAsync (user, password);

        await _context.SaveChangesAsync ();

    if (!result.Succeeded) {
      var errors = new ErrorList (result.Errors.Select (e => e.Description));
      return Result<Guid>.Error (errors);
    }

    return Result<Guid>.Success (user.Id, "Пользователь успешно создан.");
  }

  /// <summary> Проверка пароля пользователя </summary>
  public async Task<Result> CheckPasswordAsync (UserData user, string password) {
    var result = await _userManager.CheckPasswordAsync (user, password);
    if (!result)
      return Result.Error ("Пароль не правильный.");
    
    return Result.Success ();
  }

  /// <summary> Смена пароля пользователя </summary>
  public async Task<Result> ResetPasswordAsync (UserData user, string token, string newPassword) {
    var result = await _userManager.ResetPasswordAsync (user, token, newPassword);
    
    return Result.Success ();
  }

  /// <summary> Добавления роли пользователю </summary>
  public async Task<Result<string>> AddToRoleAsync (UserData user, string role) {
    var result = await _userManager.AddToRoleAsync (user, role);
    if (!result.Succeeded) {
      return Result.Error (string.Join ("; ", result.Errors.Select (e => e.Description)));
    }

    return Result<string>.Success ("Роль успешно добавлена.");
  }

  /// <summary> Получение всех ролей пользователя </summary>
  public async Task<Result<IList<string>>> GetRolesAsync (UserData user) {
    return Result<IList<string>>.Success (await _userManager.GetRolesAsync (user));
  }

  /// <summary> Получение пользователя по токену </summary>
  public async Task<Result<UserData>> GetByRefreshTokenAsync (string refreshToken) {
    var refreshTokenResult = await _refreshTokenRepository.GetValidAsync (refreshToken);
    if (!refreshTokenResult.IsSuccess)
      return Result<UserData>.Error ("Токен обновления не найден.");

    var user = await _userManager.FindByIdAsync (refreshTokenResult.Value.UserId.ToString ());
    
    return user is null
      ? Result<UserData>.NotFound ("User not found") 
      : Result<UserData>.Success (user);
  }

  /// <summary> Создание токена для сброса пароля </summary>
  public Task<string> GeneratePasswordResetTokenAsync (UserData user) =>
    _userManager.GeneratePasswordResetTokenAsync (user);

  public async Task<Result> DeleteAsync (UserData userData) {
    await _userManager.DeleteAsync(userData);
    return Result.Success ();
  }
  
  public async Task SaveChangesAsync (CancellationToken cancellationToken) {
    await _context.SaveChangesAsync (cancellationToken);
  }

}