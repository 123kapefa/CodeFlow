namespace Contracts.AuthService.Requests;

public record LoginUserRequest (string Email, string Password);