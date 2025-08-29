namespace AuthService.Application.Abstractions;

public interface ITokenService
{
  (string AccessToken, int ExpiresInSeconds)
    GenerateTokens(Guid userId, string email);
}