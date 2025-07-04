using Ardalis.Result;

using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;

using Microsoft.AspNetCore.Identity;

namespace AuthService.Infrastructure.Repositories;

public class UserDataRepository : IUserDataRepository {

  private readonly UserManager<UserData> _userManager;
  private readonly AuthServiceDbContext _context;

  public UserDataRepository (UserManager<UserData> userManager, AuthServiceDbContext context) {
    _userManager = userManager;
    _context = context;
  }

  public async Task<Result<UserData>> GetByIdAsync (Guid userId) {
    var userData = await _userManager.FindByIdAsync (userId.ToString ());

    if (userData == null) {
      Result<UserData>.NotFound ($"Пользователь с таким ID: {userId} не найден.");
    }

    return Result<UserData>.Success (userData!);
  }

  public async Task<Result<UserData>> GetByEmailAsync (string email) {
    var userData = await _userManager.FindByEmailAsync (email);

    if (userData == null) {
      Result<UserData>.NotFound ($"Пользователь с такой почтой: {email} не найден.");
    }

    return Result<UserData>.Success (userData!);
  }

  public async Task<Result<Guid>> CreateAsync (UserData user, string? password = null) {
    IdentityResult result = password is null
      ? await _userManager.CreateAsync (user)
      : await _userManager.CreateAsync (user, password);

    if (!result.Succeeded) {
      var errors = new ErrorList (result.Errors.Select (e => e.Description));
      return Result<Guid>.Error (errors);
    }

    return Result<Guid>.Success (user.Id, "Пользователь успешно создан.");
  }

  public async Task<Result<bool>> CheckPasswordAsync (UserData user, string password) {
    return Result<bool>.Success (await _userManager.CheckPasswordAsync (user, password));
  }

  public async Task<Result<string>> AddToRoleAsync (UserData user, string role) {
    var result = await _userManager.AddToRoleAsync (user, role);
    if (!result.Succeeded) {
      return Result.Error (string.Join ("; ", result.Errors.Select (e => e.Description)));
    }

    return Result<string>.Success ("Роль успешно добавлена.");
  }

  public async Task<Result<IList<string>>> GetRolesAsync (UserData user) {
    return Result<IList<string>>.Success (await _userManager.GetRolesAsync (user));
  }

  public Task<Result<UserData>> GetByRefreshTokenAsync (string refreshToken) {
    throw new NotImplementedException ();
  }

  public Task<Result> RevokeRefreshTokenAsync (string refreshToken) {
    throw new NotImplementedException ();
  }

  public Task<Result> AddRefreshTokenAsync (Guid userId, string refreshToken) {
    throw new NotImplementedException ();
  }

  public Task<string> GeneratePasswordResetTokenAsync (UserData user) =>
    _userManager.GeneratePasswordResetTokenAsync (user);

  public async Task<bool> ResetPasswordAsync (UserData user, string token, string newPassword) {
    var result = await _userManager.ResetPasswordAsync (user, token, newPassword);
    return result.Succeeded;
  }

  public async Task SaveChangesAsync () {
    await _context.SaveChangesAsync ();
  }

}