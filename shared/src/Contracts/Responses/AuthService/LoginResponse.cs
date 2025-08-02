namespace Contracts.AuthService.Responses;

public record LoginResponse(
  string AccessToken,
  string RefreshToken,
  int ExpiresInSeconds
);
