namespace AuthService.Application.Response;

public record LoginResponse(
  string AccessToken,
  string RefreshToken,
  int ExpiresInSeconds
);
