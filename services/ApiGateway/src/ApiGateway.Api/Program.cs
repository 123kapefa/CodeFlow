using ApiGateway.Api.Extensions;
using ApiGateway.Application.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder (args);

builder.Services.AddSingleton<HttpService> ();

builder.AddConfig ();
builder.AddAuth ();
builder.AddApiClientsWithResilience ();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddControllers ();

// --- CORS ---
builder.Services.AddCors(options => {
    options.AddPolicy("ReactDev", policy =>
        policy.WithOrigins(
                "http://localhost:3000",
                "http://127.0.0.1:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()); // если шлёшь cookies/авторизацию
});

builder.Services.AddHttpClient();
builder.Services.AddControllers ();

var app = builder.Build();

app.UseRouting();

// CorsMiddleware ������ ������ ����� UseRouting � UseAuth
app.UseCors("ReactDev");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Anti-spoof: стираем входящие техзаголовки от внешних клиентов
app.Use(async (ctx, next) =>
{
  ctx.Request.Headers.Remove("X-User-Id");
  ctx.Request.Headers.Remove("X-User-Name");
  ctx.Request.Headers.Remove("X-User-Roles");
  await next();
});

// После авторизации — добавляем свои X-User-* на основе Claims
app.Use(async (ctx, next) =>
{
  if (ctx.User?.Identity?.IsAuthenticated == true)
  {
    var userId = ctx.User.FindFirst("sub")?.Value
   ?? ctx.User.FindFirst("userId")?.Value;
    if (!string.IsNullOrWhiteSpace(userId))
      ctx.Request.Headers.Append("X-User-Id", userId);

    var name = ctx.User.Identity?.Name 
   ?? ctx.User.FindFirst("preferred_username")?.Value
   ?? ctx.User.FindFirst("email")?.Value;
    if (!string.IsNullOrWhiteSpace(name))
      ctx.Request.Headers.Append("X-User-Name", name);

    var roles = string.Join(',', ctx.User.FindAll("role").Select(r => r.Value));
    if (!string.IsNullOrWhiteSpace(roles))
      ctx.Request.Headers.Append("X-User-Roles", roles);
  }
  
  if (!(ctx.User?.Identity?.IsAuthenticated ?? false))
  {
    const string CookieName = "aid";
    if (!ctx.Request.Cookies.TryGetValue(CookieName, out var aid) || string.IsNullOrWhiteSpace(aid))
    {
      aid = Guid.NewGuid().ToString("N");
      ctx.Response.Cookies.Append(CookieName, aid, new CookieOptions {
        HttpOnly = true, Secure = true, SameSite = SameSiteMode.Lax,
        IsEssential = true, Expires = DateTimeOffset.UtcNow.AddYears(1)
      });
    }
  }

  await next();
});

// Применяем CORS к endpoint'у прокси (важно!)
app.MapReverseProxy().RequireCors("ReactDev");
app.MapControllers();


app.Run();