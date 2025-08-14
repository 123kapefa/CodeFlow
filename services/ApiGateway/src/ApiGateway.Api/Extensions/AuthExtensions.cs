using System.IdentityModel.Tokens.Jwt;
using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using Sprache;

namespace ApiGateway.Api.Extensions;

public static class AuthExtensions {

  public static WebApplicationBuilder AddAuth (this WebApplicationBuilder builder) {
    
    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
    jwtSettings!.Secret = Environment.GetEnvironmentVariable("JWTSETTINGS__SECRET")!;
    jwtSettings!.Issuer = Environment.GetEnvironmentVariable("JWTSETTINGS__ISSUER")!;
    jwtSettings!.Audience = Environment.GetEnvironmentVariable("JWTSETTINGS__AUDIENCE")!;
    jwtSettings!.ExpiresInMinutes = Int32.Parse(Environment.GetEnvironmentVariable("JWTSETTINGS__EXPIRESINMINUTES")!);
    
    Console.WriteLine($"JWT Secret: {jwtSettings.Secret}");
    Console.WriteLine($"JWT Issuer: {jwtSettings.Issuer}");
    Console.WriteLine($"JWT Audience: {jwtSettings.Audience}");
    Console.WriteLine($"JWT ExpiresInMinutes: {jwtSettings.ExpiresInMinutes}");

        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

        builder.Services
     .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;  // <— ВАЖНО
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,

            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),

            ClockSkew = TimeSpan.Zero,
            NameClaimType = "name",
            RoleClaimType = "role"
        };

        options.Events = new JwtBearerEvents {
            OnAuthenticationFailed = ctx =>
            {
                Console.WriteLine(ctx.Exception is SecurityTokenExpiredException
                    ? "Токен истёк!"
                    : $"Ошибка аутентификации: {ctx.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = ctx =>
            {
                // разовый дамп клеймов для диагностики
                Console.WriteLine("Token validated. Claims:");
                foreach(var c in ctx.Principal!.Claims)
                    Console.WriteLine($"  {c.Type} = {c.Value}");
                return Task.CompletedTask;
            }
        };
    });
        builder.Services.AddAuthorization(options => {
      options.AddPolicy("AuthenticatedPolicy", policy => {
        policy.RequireAuthenticatedUser();
      });
      
      options.AddPolicy("AllowAnonymousPolicy", policy => {
        policy.RequireAssertion(_ => true); // Позволяем всем
      });
    });

    
    return builder;
  }

}

public class JwtSettings
{
  public string Secret { get; set; }
  public string Issuer { get; set; }
  public string Audience { get; set; }
  public int ExpiresInMinutes { get; set; }
}
