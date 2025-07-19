using AuthService.Infrastructure;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder (args);

builder.Services.AddControllers ();

builder.Services.AddOpenApi ();
builder.Services.AddScoped<AuthServiceDbContext> (_ =>
  new AuthServiceDbContext (builder.Configuration.GetConnectionString (nameof(AuthServiceDbContext))!));

builder.Services.AddEndpointsApiExplorer ();
builder.Services.AddSwaggerGen (options => {
    options.SwaggerDoc ("v1", new OpenApiInfo {
        Title = "Product API",
        Version = "v1",
        Description = "������ ������������ Swagger ��� AuthService"
    });
});

var app = builder.Build ();

app.UseSwagger ();
app.UseSwaggerUI (options => {
    options.SwaggerEndpoint ("/swagger/v1/swagger.json", "Product API v1");
});

var app = builder.Build ();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
  app.UseSwaggerUI (options => options.SwaggerEndpoint ("/openapi/v1.json", "AuthService"));
}

app.UseHttpsRedirection ();

app.UseAuthorization ();

app.MapControllers ();

app.Run ();