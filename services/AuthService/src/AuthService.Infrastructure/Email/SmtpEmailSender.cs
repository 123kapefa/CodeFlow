using AuthService.Application.Abstractions;

namespace AuthService.Infrastructure.Email;

public class SmtpEmailSender : IEmailSender {

  public Task SendEmailAsync (string to, string subject, string htmlBody) {
    throw new NotImplementedException ();
  }

}