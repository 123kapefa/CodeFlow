namespace Contracts.AuthService.Requests;

public record EditUserDataRequest (string? Username, string? PhoneNumber);