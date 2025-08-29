
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;
using AuthService.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication;

using Microsoft.AspNetCore.Authentication.OAuth;

namespace AuthService.Api.Extensions;

public static class GithubAuthExtensions {
    public static IServiceCollection AddGithubAuth( this IServiceCollection services, IConfiguration config ) {
        var section = config.GetSection(GithubSettings.SectionName);
        services.Configure<GithubSettings>(section);

        services.AddAuthentication().AddOAuth(AuthSchemes.GitHub, options => {
            var s = new GithubSettings();
            section.Bind(s);

            options.ClientId = s.ClientId;
            options.ClientSecret = s.ClientSecret;

            // Гарантируем корректный CallbackPath с ведущим "/"
            var cb = string.IsNullOrWhiteSpace(s.CallbackPath) ? "/signin-github"
                     : (s.CallbackPath.StartsWith("/") ? s.CallbackPath : "/" + s.CallbackPath);
            options.CallbackPath = cb;

            // Эндпоинты GitHub по умолчанию (на случай, если в конфиге пусто)
            options.AuthorizationEndpoint = string.IsNullOrWhiteSpace(s.AuthorizationEndpoint) ? "https://github.com/login/oauth/authorize" : s.AuthorizationEndpoint;
            options.TokenEndpoint = string.IsNullOrWhiteSpace(s.TokenEndpoint) ? "https://github.com/login/oauth/access_token" : s.TokenEndpoint;
            options.UserInformationEndpoint = string.IsNullOrWhiteSpace(s.UserInformationEndpoint) ? "https://api.github.com/user" : s.UserInformationEndpoint;

            options.SignInScheme = AuthSchemes.ExternalCookie;

            options.CorrelationCookie.SameSite = SameSiteMode.None;
            options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;

            // cкоупы и клеймы
            options.Scope.Clear();
            options.Scope.Add("read:user");
            options.Scope.Add("user:email");

            options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
            options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
            options.ClaimActions.MapJsonKey("urn:github:login", "login");
            options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");

            // Форсируем https и голый домен в redirect_uri (как с Google)
            options.Events ??= new OAuthEvents();
            options.Events.OnRedirectToAuthorizationEndpoint = ctx => {
                var u = new Uri(ctx.RedirectUri);
                if(u.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase) &&
                    (u.Host.Equals("codeflow-project.ru", StringComparison.OrdinalIgnoreCase) ||
                     u.Host.Equals("www.codeflow-project.ru", StringComparison.OrdinalIgnoreCase))) {
                    var b = new UriBuilder(u) { Scheme = "https", Host = "codeflow-project.ru", Port = -1 };
                    ctx.Response.Redirect(b.Uri.ToString());
                }
                else {
                    ctx.Response.Redirect(ctx.RedirectUri);
                }
                return Task.CompletedTask;
            };

            // Подтягиваем профиль и email
            options.Events.OnCreatingTicket = async ctx => {
                // /user
                using(var req = new HttpRequestMessage(HttpMethod.Get, options.UserInformationEndpoint)) {
                    req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ctx.AccessToken);
                    req.Headers.UserAgent.ParseAdd("CodeFlowAuthService");
                    req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var resp = await ctx.Backchannel.SendAsync(req, ctx.HttpContext.RequestAborted);
                    resp.EnsureSuccessStatusCode();

                    using var json = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
                    ctx.RunClaimActions(json.RootElement);
                }

                var email = ctx.Principal?.FindFirstValue(ClaimTypes.Email);
                var full = ctx.Principal?.FindFirstValue(ClaimTypes.Name);

                // если email не пришёл — добираем из /user/emails
                if(string.IsNullOrWhiteSpace(email)) {
                    using var req = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user/emails");
                    req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ctx.AccessToken);
                    req.Headers.UserAgent.ParseAdd("CodeFlowAuthService");
                    req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var resp = await ctx.Backchannel.SendAsync(req, ctx.HttpContext.RequestAborted);
                    resp.EnsureSuccessStatusCode();

                    using var json = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
                    var primary = json.RootElement.EnumerateArray().FirstOrDefault(e =>
                        e.TryGetProperty("primary", out var p) && p.GetBoolean() &&
                        e.TryGetProperty("verified", out var v) && v.GetBoolean());

                    if(primary.ValueKind != JsonValueKind.Undefined &&
                        primary.TryGetProperty("email", out var eProp)) {
                        email = eProp.GetString();
                        if(!string.IsNullOrWhiteSpace(email))
                            ctx.Identity!.AddClaim(new Claim(ClaimTypes.Email, email));
                    }
                }

                if(!string.IsNullOrWhiteSpace(email)) {
                    var repo = ctx.HttpContext.RequestServices.GetRequiredService<IUserDataRepository>();
                    var existed = await repo.GetByEmailAsync(email);
                    if(!existed.IsSuccess) {
                        var user = string.IsNullOrWhiteSpace(full) ? UserData.Create(email) : UserData.Create(full, email);
                        user.EmailConfirmed = true;
                        user.IsExternal = true;
                        await repo.CreateAsync(user, password: null);
                        await repo.SaveChangesAsync(ctx.HttpContext.RequestAborted);
                    }
                }
            };

            // Доп. лог на фейлы обмена (удобно для диагностики)
            options.Events.OnRemoteFailure = ctx => {
                Console.WriteLine($"[GITHUB OAUTH FAIL] {ctx.Failure?.GetType().Name}: {ctx.Failure?.Message}");
                return Task.CompletedTask;
            };
        });

        return services;
    }
}