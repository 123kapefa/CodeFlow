using AuthService.Domain.Entities;
using AuthService.Infrastructure;
using AuthService.Infrastructure.Settings;

using Microsoft.AspNetCore.Identity;

namespace AuthService.Api.Extensions;

public static class AuthExtensions {

  public static WebApplicationBuilder AddAuth (this WebApplicationBuilder builder) {
    // builder.Services.Configure<JwtSettings> (builder.Configuration.GetSection ("JwtSettings"));

    var envPath = Path.Combine ("..", "..", "..", "..", ".env");
    DotNetEnv.Env.Load (envPath);

    builder.Services.Configure<JwtSettings> (settings =>
    {
      settings!.Secret = Environment.GetEnvironmentVariable ("JWTSETTINGS__SECRET")!;
      settings!.Issuer = Environment.GetEnvironmentVariable ("JWTSETTINGS__ISSUER")!;
      settings!.Audience = Environment.GetEnvironmentVariable ("JWTSETTINGS__AUDIENCE")!;
      settings!.ExpiresInMinutes = Int32.Parse (Environment.GetEnvironmentVariable ("JWTSETTINGS__EXPIRESINMINUTES")!);

      Console.WriteLine (settings.Secret);
      Console.WriteLine (settings.Issuer);
      Console.WriteLine (settings.Audience);
      Console.WriteLine (settings.ExpiresInMinutes);
    });


    builder.Services.AddDataProtection ();
    builder.Services.AddIdentityCore<UserData> (options =>
      {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.User.RequireUniqueEmail = true;
      }).AddRoles<IdentityRole<Guid>> ().AddEntityFrameworkStores<AuthServiceDbContext> ()
     .AddSignInManager<SignInManager<UserData>> ().AddDefaultTokenProviders ();

    return builder;
  }

  public static IApplicationBuilder UseAuth (this IApplicationBuilder app) {

    app.UseAuthorization ();
    
    return app;
  }

}