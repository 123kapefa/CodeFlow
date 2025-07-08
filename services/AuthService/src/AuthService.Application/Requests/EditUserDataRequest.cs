namespace AuthService.Application.Requests;

public record EditUserDataRequest (string? Username, string? PhoneNumber);