using AuthService.Api;
using AuthService.Api.Extensions;
using AuthService.Domain.Repositories;
using AuthService.Infrastructure;
using AuthService.Infrastructure.Repositories;
using AuthService.Infrastructure.Settings;
using Contracts.Bootstrap;
using Messaging.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;


EnvBootstrapper.Load();

var builder = WebApplication.CreateBuilder (args);

builder.AddBase ();
builder.AddAuth ();
builder.AddMail ();
builder.AddDatabase ();
builder.AddHandlers ();
builder.AddCustomSwagger ();
builder.AddCustomSerilog ();
builder.AddAuthMessaging ();
builder.Services.AddMessaging ();

// JWT settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

// Внешняя cookie (временная, на период внешнего логина)
builder.Services
  .AddAuthentication(o => {
      o.DefaultScheme = AuthSchemes.ExternalCookie;
  })
  .AddCookie(AuthSchemes.ExternalCookie, o => {
      o.ExpireTimeSpan = TimeSpan.FromMinutes(10);
      o.SlidingExpiration = false;
  });

// Подключаем провайдеров
builder.Services.AddGoogleAuth(builder.Configuration);
builder.Services.AddGithubAuth(builder.Configuration);

// Токен-сервис
builder.Services.AddScoped<IExternalTokenService, ExternalTokenService>();


var app = builder.Build ();


app.UseForwardedHeaders(new ForwardedHeadersOptions {
    ForwardedHeaders = ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedProto,
    // если в docker
    KnownNetworks = { },  // иначе X-Forwarded-* могут игнориться
    KnownProxies = { }
});

app.UseCustomSwagger ();
app.UseBase ();

using (var scope = app.Services.CreateScope())
{
  var services = scope.ServiceProvider;
  try
  {
    var context = services.GetRequiredService<AuthServiceDbContext>();
    context.Database.Migrate();
  }
  catch (Exception ex)
  {
    Console.WriteLine($"Ошибка при выполнении миграций: {ex.Message}");
    throw;
  }
}

app.UseAuth ();
app.MapControllers ();

app.Run ();