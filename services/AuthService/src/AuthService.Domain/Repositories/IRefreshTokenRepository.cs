using Ardalis.Result;

using AuthService.Domain.Entities;

namespace AuthService.Domain.Repositories;

public interface IRefreshTokenRepository {
  /// <summary>Создать и сохранить новый refresh-токен для пользователя.</summary>
  Task<Result<RefreshToken>> CreateAsync(Guid userId, TimeSpan ttl);
  
  /// <summary>Найти не отозванный, не просроченный токен.</summary>
  Task<Result<RefreshToken>> GetValidAsync(string token);
  
  /// <summary>Отозвать конкретный токен.</summary>
  Task<Result> RevokeAsync(string token);

  /// <summary>Отозвать все токены пользователя.</summary>
  Task<Result> RevokeAllAsync(Guid userId);

}