namespace Contracts.AuthService.Responses;

public record RefreshTokenResponse(string AccessToken, string RefreshToken, int ExpiresInSeconds);