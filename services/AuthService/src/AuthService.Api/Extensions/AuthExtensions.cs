using AuthService.Domain.Entities;
using AuthService.Infrastructure;
using AuthService.Infrastructure.Settings;

using Microsoft.AspNetCore.Identity;

namespace AuthService.Api.Extensions;

public static class AuthExtensions {

  public static WebApplicationBuilder AddAuth (this WebApplicationBuilder builder) {
      
        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

        builder.Services.AddDataProtection();

        builder.Services.AddIdentityCore<UserData>(o => {
            o.Password.RequireDigit = true;
            o.Password.RequiredLength = 6;
            o.User.RequireUniqueEmail = true;
        })
       .AddRoles<IdentityRole<Guid>>()
       .AddEntityFrameworkStores<AuthServiceDbContext>()
       .AddSignInManager<SignInManager<UserData>>()
       .AddDefaultTokenProviders();

    return builder;
  }

  public static IApplicationBuilder UseAuth (this IApplicationBuilder app) {

    app.UseAuthorization ();
    
    return app;
  }

}