using ApiGateway.Api.Extensions;
using ApiGateway.Application.Services;
using Contracts.Bootstrap;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Yarp.ReverseProxy.Transforms;


EnvBootstrapper.Load();


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
            "http://127.0.0.1:3000",
            "https://codeflow-project.ru"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()); // если шлёшь cookies/авторизацию

});


var app = builder.Build();

app.UseRouting();

// CorsMiddleware ������ ������ ����� UseRouting � UseAuth
app.UseCors("ReactDev");


// доверяем заголовкам от reverse-proxy (nginx)
var fwd = new ForwardedHeadersOptions {
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
// для локалки убираем ограничение «доверенных прокси»
fwd.KnownNetworks.Clear();
fwd.KnownProxies.Clear();

app.UseForwardedHeaders(fwd);


app.UseAuthentication();
app.UseAuthorization();
app.Use(async ( ctx, next ) => {
    Console.WriteLine($"GW IsAuth={ctx.User?.Identity?.IsAuthenticated}, sub={ctx.User?.FindFirst("sub")?.Value}");
    await next();
});

// Применяем CORS к endpoint'у прокси (важно!)
app.MapReverseProxy().RequireCors("ReactDev");
app.MapControllers();



app.Run();