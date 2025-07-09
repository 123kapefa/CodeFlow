namespace AuthService.Infrastructure.Settings;

public class GoogleAuthSettings {

  public const string SectionName = "Authentication:Google";

  public string ClientId { get; set; } = null!;
  public string ClientSecret { get; set; } = null!;

}