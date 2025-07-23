namespace Contracts.AuthService.Requests;

public record EmailChangeRequest (
  string OldEmail,
  string NewEmail,
  string ConfirmNewEmail
  );