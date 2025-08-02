namespace Contracts.AuthService.Requests;

public record RegisterUserRequest (string Username, string Email, string Password);