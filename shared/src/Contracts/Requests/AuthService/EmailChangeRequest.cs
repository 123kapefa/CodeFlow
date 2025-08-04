namespace Contracts.Requests.AuthService;

public record EmailChangeRequest (
  string OldEmail,
  string NewEmail,
  string ConfirmNewEmail
  );