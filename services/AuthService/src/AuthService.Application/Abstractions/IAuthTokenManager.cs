using Ardalis.Result;

namespace AuthService.Application.Abstractions;

public interface IAuthTokenManager {

  /// <summary>Генерирует access-JWT и сохраняет refresh-токен в БД.</summary>
  Task<Result<(string AccessToken, string RefreshToken, int ExpiresInSeconds)>>
    CreateTokensAsync(Guid userId, string email, TimeSpan refreshTtl);

  /// <summary>По refresh-токену выдаёт новый access + refresh (rotating tokens).</summary>
  Task<Result<(string AccessToken, string RefreshToken, int ExpiresInSeconds)>> 
    RefreshTokensAsync(string refreshToken, TimeSpan refreshTtl);

  /// <summary>Отзывает конкретный refresh-токен.</summary>
  Task<Result> RevokeRefreshAsync(string refreshToken);

}