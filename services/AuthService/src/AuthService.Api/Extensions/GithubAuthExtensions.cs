using System.Security.Claims;
using System.Text.Json;

using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;
using AuthService.Infrastructure.Settings;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace AuthService.Api.Extensions;

public static class GithubAuthExtensions {

  public static IServiceCollection AddGithubAuth (this IServiceCollection services, IConfiguration config) {
    
    var section = config.GetSection (GithubSettings.SectionName);
    services.Configure<GithubSettings> (section);

    services.AddAuthentication ().AddOAuth ("GitHub", options =>
    {
      section.Bind (options);
      options.Scope.Add ("user:email");
      options.ClaimActions.MapJsonKey (ClaimTypes.NameIdentifier, "id");
      options.ClaimActions.MapJsonKey (ClaimTypes.Name, "name");
      options.ClaimActions.MapJsonKey (ClaimTypes.Email, "email");
      options.Events = new OAuthEvents {
        OnCreatingTicket = async ctx =>
        {
          var email = ctx.Principal.FindFirstValue (ClaimTypes.Email)!;
          var repo = ctx.HttpContext.RequestServices.GetRequiredService<IUserDataRepository> ();
          var userRes = await repo.GetByEmailAsync (email);
          if (!userRes.IsSuccess) {
            var user = UserData.Create (email);
            await repo.CreateAsync (user, password: null);
            await repo.SaveChangesAsync ();
          }
        }
      };
    });

    return services;
  }

}