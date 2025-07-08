namespace AuthService.Application.Requests;

public record EditUserDataRequest (string? Fullname, string? PhoneNumber);