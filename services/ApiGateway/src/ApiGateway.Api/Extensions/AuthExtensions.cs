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
    
    builder.Services
     .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
     .AddJwtBearer(options => {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = jwtSettings.Issuer,
          ValidAudience = jwtSettings.Audience,
          IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.Secret)
          )
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
