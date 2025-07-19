namespace AuthService.Application.Response;

public record RefreshTokenResponse(string AccessToken, string RefreshToken, int ExpiresInSeconds);