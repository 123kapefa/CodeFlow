namespace AuthService.Application.Abstractions;

public interface IEmailSender {

  Task SendEmailAsync (string to, string subject, string htmlBody);

}