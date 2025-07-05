using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder (args);

builder.Services
 .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
  {
    options.Authority = builder.Configuration["Jwt:Authority"];
    options.Audience  = builder.Configuration["Jwt:Audience"];
    options.RequireHttpsMetadata = false;
  });

builder.Services
 .AddReverseProxy()
 .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy()
 .RequireAuthorization();

app.Run();