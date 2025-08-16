using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
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
           .GetSection("JwtSettings")
           .Get<JwtSettings>() ?? new JwtSettings();
        
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
        services.Configure<JwtSettings>(config.GetSection("JwtSettings"));
        return services;
    }

    public static WebApplication UseJwtAuth( this WebApplication app ) {
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }
}

public sealed class JwtSettings {
    public string Secret { get; set; } = default!;
    public string Issuer { get; set; } = "";
    public string Audience { get; set; } = "";
    public int ExpiresInMinutes { get; set; } = 60;
}