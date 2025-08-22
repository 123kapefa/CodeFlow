namespace AuthService.Infrastructure.Settings;

public class JwtSettings {
  public string Secret { get; set; }
  public string Issuer { get; set; }
  public string Audience { get; set; }
  public int ExpiresInMinutes { get; set; }

    public const string SectionName = "JwtSettings";
    public bool RequireHttps { get; set; } = true;
}