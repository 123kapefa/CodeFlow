namespace Contracts.AuthService.Requests;

public record PasswordChangeRequest (string OldPassword, string NewPassword, string ConfirmNewPassword);