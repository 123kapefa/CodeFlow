using Ardalis.Result;

using AuthService.Application.Abstractions;
using AuthService.Domain.Repositories;

namespace AuthService.Infrastructure.Security;

public class AuthTokenManager : IAuthTokenManager {

  private readonly ITokenService _tokenService;
  private readonly IRefreshTokenRepository _refreshTokenRepository;

  public AuthTokenManager (ITokenService tokenService, IRefreshTokenRepository refreshTokenRepository) {
    _tokenService = tokenService;
    _refreshTokenRepository = refreshTokenRepository;
  }

  public async Task<Result<(string AccessToken, string RefreshToken, int ExpiresInSeconds)>> CreateTokensAsync (
    Guid userId
    , string email
    , TimeSpan refreshTtl) {
    // 1) Генерация access-токена
    var (accessToken, expires) = _tokenService.GenerateTokens (userId, email);
    
    // 2) Сохранение refresh-токена
    var tokensResult = await _refreshTokenRepository.CreateAsync (userId, refreshTtl);
    
    if (!tokensResult.IsSuccess)
      return Result<(string, string, int)>.Error (new ErrorList (tokensResult.Errors));

    return Result<(string AccessToken, string RefreshToken, int ExpiresInSeconds)>
     .Success ((accessToken, tokensResult.Value.Token, expires));
  }

  public async Task<Result<(string AccessToken, string RefreshToken, int ExpiresInSeconds)>> RefreshTokensAsync (
    string refreshToken
    , TimeSpan refreshTtl) {
    // 1) Проверяем валидность старого
    var validRefreshToken = await _refreshTokenRepository.GetValidAsync (refreshToken);
    if (!validRefreshToken.IsSuccess)
      return Result<(string, string, int)>.Error (new ErrorList(validRefreshToken.Errors));

    // 2) Отзываем старый
    await _refreshTokenRepository.RevokeAsync (refreshToken);

    // 3) Генерим и сохраняем новые
    var userId = validRefreshToken.Value.UserId;
    var email = validRefreshToken.Value.User?.Email ?? throw new InvalidOperationException ();
    var tokensResult = await CreateTokensAsync (userId, email, refreshTtl);
    
    if (!tokensResult.IsSuccess)
      return Result<(string, string, int)>.Error (new ErrorList (tokensResult.Errors));
    
    return Result<(string, string, int)>.Success (tokensResult);
  }

  public Task<Result> RevokeRefreshAsync (string refreshToken) => _refreshTokenRepository.RevokeAsync (refreshToken);

}