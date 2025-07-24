using ApiGateway.Api.Extensions;

using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder (args);

builder.AddConfig ();
builder.AddAuth ();

var app = builder.Build();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy ();

app.Run();