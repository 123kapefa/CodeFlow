
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
            options.CallbackPath = s.CallbackPath;
            options.AuthorizationEndpoint = s.AuthorizationEndpoint;
            options.TokenEndpoint = s.TokenEndpoint;
            options.UserInformationEndpoint = s.UserInformationEndpoint;
            options.SignInScheme = AuthSchemes.ExternalCookie;

            options.CorrelationCookie.SameSite = SameSiteMode.None;
            options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;

            options.Scope.Add("read:user");
            options.Scope.Add("user:email");

            options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
            options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
            options.ClaimActions.MapJsonKey("urn:github:login", "login");
            options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");

            options.Events = new OAuthEvents {
                OnCreatingTicket = async ctx => {
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

                    // ���� email �� ������ � /user/emails (���� primary+verified)
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
                            var user = string.IsNullOrWhiteSpace(full)
                                ? UserData.Create(email)
                                : UserData.Create(full, email);

                            user.EmailConfirmed = true;
                            user.IsExternal = true;

                            await repo.CreateAsync(user, password: null);
                            await repo.SaveChangesAsync(ctx.HttpContext.RequestAborted);
                        }
                    }
                }
            };
        });

        return services;
    }
}