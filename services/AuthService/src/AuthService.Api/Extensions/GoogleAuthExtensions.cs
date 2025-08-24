using System.Security.Claims;
using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;
using AuthService.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication;

namespace AuthService.Api.Extensions;

public static class GoogleAuthExtensions {

    public static IServiceCollection AddGoogleAuth( this IServiceCollection services, IConfiguration config ) {
        services.Configure<GoogleAuthSettings>(config.GetSection("Authentication:Google"));

        services.AddAuthentication().AddGoogle(AuthSchemes.Google, options => {
            // Bind может затереть дефолтные scopes/claims — поэтому переопределяем ниже
            config.GetSection("Authentication:Google").Bind(options);

            options.SignInScheme = AuthSchemes.ExternalCookie;
            options.CallbackPath = "/signin-google";
            options.SaveTokens = true;

            options.CorrelationCookie.SameSite = SameSiteMode.None;
            options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;

            // гарантируем нужные scope
            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("email");

            // на всякий случай нормализуем клеймы
            options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
            options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
            options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");

            options.Events.OnCreatingTicket = async ctx => {
                var email = ctx.Principal?.FindFirstValue(ClaimTypes.Email);
                if(string.IsNullOrWhiteSpace(email)) return;

                var repo = ctx.HttpContext.RequestServices.GetRequiredService<IUserDataRepository>();
                var userRes = await repo.GetByEmailAsync(email);
                if(!userRes.IsSuccess) {
                    var name = ctx.Principal!.FindFirstValue(ClaimTypes.Name);
                    var user = string.IsNullOrWhiteSpace(name) ? UserData.Create(email) : UserData.Create(name, email);
                    user.EmailConfirmed = true;
                    user.IsExternal = true;
                    await repo.CreateAsync(user, password: null);
                    await repo.SaveChangesAsync(ctx.HttpContext.RequestAborted);
                }
            };
        });

        return services;
    }
}