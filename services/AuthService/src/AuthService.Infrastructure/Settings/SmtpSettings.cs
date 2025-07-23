namespace AuthService.Infrastructure.Settings;

public class SmtpSettings
{
  public const string SectionName = "SmtpSettings";

  public string Host { get; set; } = null!;
  public int Port { get; set; }
  public string From { get; set; } = null!;
  public string Username { get; set; } = null!;
  public string Password { get; set; } = null!;
  public bool EnableSsl { get; set; } = true;
}