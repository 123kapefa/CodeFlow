namespace Contracts.Requests.AuthService;

public record PasswordChangeRequest (string OldPassword, string NewPassword, string ConfirmNewPassword);