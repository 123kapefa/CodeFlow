namespace AuthService.Application.Requests;

public record EmailChangeConfirmRequest (string NewEmail, string Token); 