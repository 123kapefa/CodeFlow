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


var app = builder.Build();

app.UseRouting();

// CorsMiddleware должен стоять между UseRouting и UseAuth
app.UseCors("ReactDev");

app.UseAuthentication();
app.UseAuthorization();


// Применяем CORS к endpoint'у прокси (важно!)
app.MapReverseProxy().RequireCors("ReactDev");
app.MapControllers();


app.Run();