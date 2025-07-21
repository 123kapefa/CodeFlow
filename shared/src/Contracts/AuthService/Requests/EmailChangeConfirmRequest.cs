namespace Contracts.AuthService.Requests;

public record EmailChangeConfirmRequest (string NewEmail, string Token); 