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

builder.Services.AddHttpClient();
builder.Services.AddControllers ();

var app = builder.Build();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapReverseProxy ();

app.Run();