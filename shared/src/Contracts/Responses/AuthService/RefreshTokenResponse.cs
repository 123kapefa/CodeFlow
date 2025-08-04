namespace Contracts.Responses.AuthService;

public record RefreshTokenResponse(string AccessToken, string RefreshToken, int ExpiresInSeconds);