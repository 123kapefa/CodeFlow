namespace Contracts.Requests.AuthService;

public record PasswordChangeConfirmRequest ( string Email, string Token);