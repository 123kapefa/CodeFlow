namespace Contracts.Requests.AuthService;

public record EmailChangeConfirmRequest (string NewEmail, string Token); 