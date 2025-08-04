using System.Security.Claims;

using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;
using AuthService.Infrastructure.Settings;

using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Api.Extensions;

public static class GoogleAuthExtensions {
  
  public static IServiceCollection AddGoogleAuth (this IServiceCollection services, IConfiguration config) {
    services.Configure<GoogleAuthSettings> (config.GetSection ("Authentication:Google"));
    services.AddAuthentication ().AddGoogle (options =>
    {
      config.GetSection ("Authentication:Google").Bind (options);
      options.Events.OnCreatingTicket = async ctx =>
      {
        var email = ctx.Principal!.FindFirstValue (ClaimTypes.Email)!;
        var repo = ctx.HttpContext.RequestServices.GetRequiredService<IUserDataRepository> ();
        var userRes = await repo.GetByEmailAsync (email);
        if (!userRes.IsSuccess) {
          var user = UserData.Create (email);
          await repo.CreateAsync (user, password: null);
          await repo.SaveChangesAsync (new CancellationToken(false));
        }
      };
    });
    return services;
  }

}