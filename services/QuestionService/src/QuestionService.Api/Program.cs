using Auth.Extensions;
using Contracts.Bootstrap;
using Messaging.Extensions;
using Microsoft.EntityFrameworkCore;
using QuestionService.Api.Extensions;
using QuestionService.Application.Abstractions;
using QuestionService.Infrastructure.Data;
using QuestionService.Infrastructure.Repositories;
using StackExchange.Redis;

EnvBootstrapper.Load();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddBase ();
builder.AddCustomSwagger ();
builder.AddDatabase ();
builder.AddCustomSerilog ();
builder.AddHandlers ();
builder.AddQuestionMessaging ();
builder.Services.AddMessaging();

builder.Services.AddControllers();

var app = builder.Build();

app.UseCustomSwagger ();
app.UseBase ();

app.Use(async (ctx, next) =>
{
    if (!ctx.User.Identity?.IsAuthenticated ?? true)
    {
        const string CookieName = "aid";
        if (!ctx.Request.Cookies.TryGetValue(CookieName, out var aid) || string.IsNullOrWhiteSpace(aid))
        {
            aid = Guid.NewGuid().ToString("N");
            ctx.Response.Cookies.Append(
                CookieName,
                aid,
                new CookieOptions {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    IsEssential = true,
                    Expires = DateTimeOffset.UtcNow.AddYears(1)
                });
        }
    }
    await next();
});

using(var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;
    try {
        var context = services.GetRequiredService<QuestionServiceDbContext>();
        context.Database.Migrate();
    }
    catch(Exception ex) {
        Console.WriteLine($"������ ��� ���������� ��������: {ex.Message}");
        throw;
    }
}

app.UseJwtAuth();

app.MapControllers ();

app.Run();