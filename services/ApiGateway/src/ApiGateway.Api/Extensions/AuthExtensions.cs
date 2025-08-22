using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


namespace ApiGateway.Api.Extensions;

public static class AuthExtensions {

  public static WebApplicationBuilder AddAuth (this WebApplicationBuilder builder) {

        var jwt = new JwtSettings();
        builder.Configuration.GetSection("JwtSettings").Bind(jwt);

        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

        builder.Services
      .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(options => {
          options.MapInboundClaims = false;
          options.RequireHttpsMetadata = builder.Environment.IsProduction();

          options.TokenValidationParameters = new TokenValidationParameters {
              ValidateIssuer = true,
              ValidIssuer = jwt.Issuer,
              ValidateAudience = true,
              ValidAudience = jwt.Audience,
              ValidateLifetime = true,
              ValidateIssuerSigningKey = true,
              IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret)),
              ClockSkew = TimeSpan.Zero,
              NameClaimType = "name",
              RoleClaimType = "role"
          };
      });


        builder.Services.AddAuthorization(options => {
            options.AddPolicy("AuthenticatedPolicy", p => p.RequireAuthenticatedUser());
            options.AddPolicy("AllowAnonymousPolicy", p => p.RequireAssertion(_ => true));
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
