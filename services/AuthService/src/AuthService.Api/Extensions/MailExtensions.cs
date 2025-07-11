using AuthService.Application.Abstractions;
using AuthService.Infrastructure.Email;
using AuthService.Infrastructure.Settings;

namespace AuthService.Api.Extensions;

public static class MailExtensions {

  public static WebApplicationBuilder UseMail (this WebApplicationBuilder builder) {

    builder.Services.Configure<SmtpSettings> (builder.Configuration.GetSection (SmtpSettings.SectionName));

    builder.Services.AddScoped<IEmailSender, SmtpEmailSender> ();
    return builder;
    
  }
}