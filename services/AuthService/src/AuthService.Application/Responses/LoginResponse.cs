namespace AuthService.Application.Responses;

public record LoginResponse(
  string AccessToken,
  string RefreshToken,
  int ExpiresInSeconds
);
