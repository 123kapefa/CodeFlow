namespace Contracts.Requests.AuthService;

public record RegisterUserRequest (string Username, string Email, string Password);