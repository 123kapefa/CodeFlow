namespace AuthService.Application.Abstractions;

/// <summary>
/// Генерирует access- и refresh-токены.
/// </summary>
public interface ITokenService
{
  /// <param name="userId">ID пользователя</param>
  /// <param name="email">Email (или другой уникальный claim)</param>
  /// <returns>
  /// Tuple: AccessToken, RefreshToken, expiresInSeconds
  /// </returns>
  (string AccessToken, string RefreshToken, int ExpiresInSeconds)
    GenerateTokens(Guid userId, string email);
}