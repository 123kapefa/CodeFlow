using ApiGateway.Api.Extensions;

using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder (args);

builder.AddConfig ();
builder.AddAuth ();

// --- CORS ---
builder.Services.AddCors(options => {
    options.AddPolicy("ReactDev", policy =>
        policy.WithOrigins(
                "http://localhost:3000",
                "http://127.0.0.1:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()); // ���� ���� cookies/�����������
});

var app = builder.Build();

app.UseRouting();

// CorsMiddleware ������ ������ ����� UseRouting � UseAuth
app.UseCors("ReactDev");

app.UseAuthentication();
app.UseAuthorization();

// ��������� CORS � endpoint'� ������ (�����!)
app.MapReverseProxy().RequireCors("ReactDev");

app.Run();