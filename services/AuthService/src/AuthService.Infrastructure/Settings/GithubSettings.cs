namespace AuthService.Infrastructure.Settings;

public class GithubSettings {

  public const string SectionName = "Authentication:Github";

  public string ClientId { get; set; } = null!;
  public string ClientSecret { get; set; } = null!;
  public string CallbackPath { get; set; } = null!;
  public string AuthorizationEndpoint { get; set; } = null!;
  public string TokenEndpoint { get; set; } = null!;
  public string UserInformationEndpoint { get; set; } = null!;

}