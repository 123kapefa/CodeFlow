using Ardalis.Result;

using AuthService.Domain.Entities;

namespace AuthService.Domain.Repositories;

public interface IUserDataRepository {

  Task<Result<UserData>> GetByIdAsync(Guid userId);
  Task<Result<UserData>> GetByEmailAsync(string email);
  Task<Result<Guid>> CreateAsync(UserData user, string? password = null);
  Task<Result<bool>> CheckPasswordAsync(UserData user, string password);
  Task<Result<string>> AddToRoleAsync(UserData user, string role);
  Task<Result<IList<string>>> GetRolesAsync(UserData user);
  Task<Result<UserData>> GetByRefreshTokenAsync (string refreshToken);
  Task<Result> RevokeRefreshTokenAsync (string refreshToken);
  Task<Result> AddRefreshTokenAsync (Guid userId, string refreshToken);
  /// <summary>Сгенерировать токен для сброса пароля (Identity).</summary>
  Task<string> GeneratePasswordResetTokenAsync(UserData user);

  /// <summary>Сбросить пароль по токену (Identity). Возвращает true, если прошло успешно.</summary>
  Task<bool> ResetPasswordAsync(UserData user, string token, string newPassword);
  Task SaveChangesAsync();

}