using System.Net;
using System.Net.Mail;

using AuthService.Application.Abstractions;
using AuthService.Infrastructure.Settings;

using Microsoft.Extensions.Options;

namespace AuthService.Infrastructure.Email;

public class SmtpEmailSender : IEmailSender {

  private readonly SmtpSettings _settings;

  public SmtpEmailSender (IOptions<SmtpSettings> options) {
    _settings = options.Value;
  }

  public async Task SendEmailAsync (string to, string subject, string htmlBody) {
    using var client = new SmtpClient (_settings.Host, _settings.Port) {
      EnableSsl = _settings.EnableSsl, Credentials = new NetworkCredential (_settings.Username, _settings.Password)
    };

    var mail = new MailMessage {
      From = new MailAddress (_settings.From), Subject = subject, Body = htmlBody, IsBodyHtml = true
    };
    mail.To.Add (to);

    await client.SendMailAsync (mail);
  }

}