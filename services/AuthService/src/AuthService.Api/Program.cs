using AuthService.Api.Extensions;
using AuthService.Infrastructure;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder (args);

builder.AddBase ();
builder.AddAuth ();
builder.AddMail ();
builder.AddDatabase ();
builder.AddHandlers ();
builder.AddCustomSwagger ();
builder.AddCustomSerilog ();

var app = builder.Build ();

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