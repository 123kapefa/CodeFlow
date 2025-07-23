using AuthService.Domain.Entities;
using AuthService.Infrastructure;

using Microsoft.AspNetCore.Identity;

namespace AuthService.Api.Extensions;

public static class AuthExtensions {

  public static WebApplicationBuilder AddAuth (this WebApplicationBuilder builder) {
    
    builder.Services.AddDataProtection();
    builder.Services.AddIdentityCore<UserData>(options => {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.User.RequireUniqueEmail = true;
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