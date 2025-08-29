using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;

using Auth.Jwt;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Extensions;

public static class AuthorizationExtensions {

    public static IServiceCollection AddJwtAuth(
        this IServiceCollection services,
        IConfiguration config ) {

        var settings = config
           .GetSection("JwtOptions")
           .Get<JwtOptions>() ?? new JwtOptions();
        
        settings.Secret = Environment.GetEnvironmentVariable("JWTSETTINGS__SECRET") ?? settings.Secret;
        settings.Issuer = Environment.GetEnvironmentVariable("JWTSETTINGS__ISSUER") ?? settings.Issuer;
        settings.Audience = Environment.GetEnvironmentVariable("JWTSETTINGS__AUDIENCE") ?? settings.Audience;
        settings.ExpiryMinutes = int.Parse(Environment.GetEnvironmentVariable("JWTSETTINGS__EXPIRESINMINUTES")!);
        
        Console.WriteLine(JsonSerializer.Serialize(settings));
        
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
          .AddJwtBearer(options => {
              options.MapInboundClaims = false;
              options.RequireHttpsMetadata = false;

              options.TokenValidationParameters = new TokenValidationParameters {
                  ValidateIssuer = !string.IsNullOrWhiteSpace(settings.Issuer),
                  ValidIssuer = settings.Issuer,

                  ValidateAudience = !string.IsNullOrWhiteSpace(settings.Audience),
                  ValidAudience = settings.Audience,

                  ValidateIssuerSigningKey = true,
                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret)),

                  ValidateLifetime = true,
                  ClockSkew = TimeSpan.Zero,

                  NameClaimType = "name",
                  RoleClaimType = "role"
              };
          });

        services.AddAuthorization();
        services.Configure<JwtOptions>(config.GetSection("JwtOptions"));

        return services;
    }

    public static WebApplication UseJwtAuth( this WebApplication app ) {
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }
}

// public sealed class JwtSettings {
//     public string Secret { get; set; } = default!;
//     public string Issuer { get; set; } = "";
//     public string Audience { get; set; } = "";
//     public int? ExpiresInMinutes { get; set; }
// }

