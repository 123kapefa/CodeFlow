namespace AuthService.Infrastructure.Settings;

public class GithubSettings {
    public const string SectionName = "Authentication:GitHub";

    public string ClientId { get; set; } = default!;
    public string ClientSecret { get; set; } = default!;
    public string CallbackPath { get; set; } = "/signin-github";
    public string AuthorizationEndpoint { get; set; } = "https://github.com/login/oauth/authorize";
    public string TokenEndpoint { get; set; } = "https://github.com/login/oauth/access_token";
    public string UserInformationEndpoint { get; set; } = "https://api.github.com/user";

}