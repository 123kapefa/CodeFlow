namespace Contracts.Requests.AuthService;

public record LoginUserRequest (string Email, string Password);