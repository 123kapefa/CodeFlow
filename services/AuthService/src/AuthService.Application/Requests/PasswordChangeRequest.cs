namespace AuthService.Application.Requests;

public record PasswordChangeRequest (string OldPassword, string NewPassword, string ConfirmNewPassword);