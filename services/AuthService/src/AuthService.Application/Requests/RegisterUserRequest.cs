namespace AuthService.Application.Requests;

public record RegisterUserRequest (string Username, string Email, string Password);