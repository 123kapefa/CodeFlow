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
        IConfiguration config,
        string sectionName = "JwtSettings" ) {

        var settings = new JwtSettings();


        Console.WriteLine(Directory.GetCurrentDirectory());

        var envPath = Path.Combine("..", "..", "..", "..", ".env");

        Console.WriteLine(envPath);

        DotNetEnv.Env.Load(envPath);

        var configurationBuilder = new ConfigurationBuilder()
         .SetBasePath(Directory.GetCurrentDirectory())       
         .AddEnvironmentVariables();
        

        // Перекрываем переменные окружения, если заданы
        settings.Secret = Environment.GetEnvironmentVariable("JWTSETTINGS__SECRET") ?? settings.Secret;
        settings.Issuer = Environment.GetEnvironmentVariable("JWTSETTINGS__ISSUER") ?? settings.Issuer;
        settings.Audience = Environment.GetEnvironmentVariable("JWTSETTINGS__AUDIENCE") ?? settings.Audience;

        Console.WriteLine("++++++++++++++++");
        Console.WriteLine("++++++++++++++++");
        Console.WriteLine(JsonSerializer.Serialize(settings));
        Console.WriteLine("++++++++++++++++");
        Console.WriteLine("++++++++++++++++");

        //if(string.IsNullOrWhiteSpace(settings.Secret))
        //    throw new InvalidOperationException("JWT Secret is not configured.");

        //// Не ремапим клеймы: 'sub' останется 'sub'
        //JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

        //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //  .AddJwtBearer(options => {
        //      options.MapInboundClaims = false;
        //      options.RequireHttpsMetadata = false; // включи true в проде на HTTPS

        //      options.TokenValidationParameters = new TokenValidationParameters {
        //          ValidateIssuer = !string.IsNullOrWhiteSpace(settings.Issuer),
        //          ValidIssuer = settings.Issuer,

        //          ValidateAudience = !string.IsNullOrWhiteSpace(settings.Audience),
        //          ValidAudience = settings.Audience,

        //          ValidateIssuerSigningKey = true,
        //          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret)),

        //          ValidateLifetime = true,
        //          ClockSkew = TimeSpan.Zero,

        //          NameClaimType = "name",
        //          RoleClaimType = "role"
        //      };
        //  });

        //services.AddAuthorization();
        //services.Configure<JwtSettings>(config.GetSection(sectionName));
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