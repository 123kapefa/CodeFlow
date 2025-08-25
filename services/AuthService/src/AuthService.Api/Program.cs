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
using System.Net;


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
      o.Cookie.Name = "cf_ext";
      o.Cookie.SameSite = SameSiteMode.None;            // критично за HTTPS на другом домене
      o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
      o.ExpireTimeSpan = TimeSpan.FromMinutes(10);
      o.SlidingExpiration = false;
  });

// Подключаем провайдеров
builder.Services.AddGoogleAuth(builder.Configuration);
builder.Services.AddGithubAuth(builder.Configuration);

// Токен-сервис
builder.Services.AddScoped<IExternalTokenService, ExternalTokenService>();


var app = builder.Build ();

var fwd = new ForwardedHeadersOptions {
    ForwardedHeaders = ForwardedHeaders.XForwardedProto
                     | ForwardedHeaders.XForwardedHost
                     | ForwardedHeaders.XForwardedFor,
    ForwardLimit = 2, // для /signin-* цепочка: nginx -> authservice
    RequireHeaderSymmetry = false
};

//fwd.KnownNetworks.Clear();
//fwd.KnownProxies.Clear();

//fwd.KnownNetworks.Add(new Microsoft.AspNetCore.HttpOverrides.IPNetwork(IPAddress.Parse("172.18.0.0"), 16));
fwd.KnownNetworks.Add(new Microsoft.AspNetCore.HttpOverrides.IPNetwork(IPAddress.Parse("172.18.0.0"), 16));
fwd.KnownNetworks.Add(new Microsoft.AspNetCore.HttpOverrides.IPNetwork(IPAddress.Parse("::ffff:0:0"), 96));

fwd.KnownProxies.Add(IPAddress.Parse("172.18.0.22"));
fwd.KnownProxies.Add(IPAddress.Parse("::ffff:172.18.0.22"));

app.UseForwardedHeaders(fwd);


app.UseWhen(ctx => ctx.Request.Path.StartsWithSegments("/signin-"), branch => {
    branch.Use((ctx, next) => {
        ctx.Request.Scheme = "https";
        ctx.Request.Host   = new HostString("codeflow-project.ru");
        return next();
    });
});


app.Use(( ctx, next ) => {
    if(ctx.Request.Path.Equals("/signin-google", StringComparison.OrdinalIgnoreCase)) {
        Console.WriteLine($"[CBK] scheme={ctx.Request.Scheme}, host={ctx.Request.Host}, xfp={ctx.Request.Headers["X-Forwarded-Proto"]}, xfh={ctx.Request.Headers["X-Forwarded-Host"]}, xfpn={ctx.Request.Headers["X-Forwarded-Port"]}");
    }
    return next();

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