using Ardalis.Result;

using AuthService.Domain.Entities;

namespace AuthService.Domain.Repositories;

public interface IUserDataRepository {
  /// <summary> Получение пользователя по ID </summary>
  Task<Result<UserData>> GetByIdAsync(Guid userId);
  /// <summary> Получение пользователя по Email </summary>
  Task<Result<UserData>> GetByEmailAsync(string email);
  Task<Result<Guid>> CreateAsync(UserData user, string? password = null);
  Task<Result> CheckPasswordAsync(UserData user, string password);
  Task<Result<string>> AddToRoleAsync(UserData user, string role);
  Task<Result<IList<string>>> GetRolesAsync(UserData user);
  Task<Result<UserData>> GetByRefreshTokenAsync (string refreshToken);
  Task<string> GeneratePasswordResetTokenAsync(UserData user);
  Task<Result> ResetPasswordAsync(UserData user, string token, string newPassword);
  Task<Result> DeleteAsync (UserData userData);
  Task SaveChangesAsync(CancellationToken cancellationToken);

}