namespace Contracts.Responses.AuthService;

public record LoginResponse(
  string AccessToken,
  string RefreshToken,
  int ExpiresInSeconds
);
