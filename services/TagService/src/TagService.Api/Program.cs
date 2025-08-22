using Contracts.Bootstrap;
using MassTransit;
using Messaging.Extensions;
using Microsoft.EntityFrameworkCore;
using TagService.Api.Extensions;
using TagService.Infrastructure.Data;


EnvBootstrapper.Load();


var builder = WebApplication.CreateBuilder(args);

builder.AddBase ();
builder.AddCustomSwagger ();
builder.AddDatabase ();
builder.AddCustomSerilog ();
builder.AddHandlers ();
builder.AddTagMessaging ();
builder.Services.AddMessaging ();

builder.Services.AddControllers();

var app = builder.Build();

app.UseCustomSwagger ();
app.UseBase ();

using (var scope = app.Services.CreateScope())
{
  var services = scope.ServiceProvider;
  try
  {
    var context = services.GetRequiredService<TagServiceDbContext>();
    context.Database.Migrate();
  }
  catch (Exception ex)
  {
    Console.WriteLine($"Ошибка при выполнении миграций: {ex.Message}");
    throw;
  }
}

app.MapControllers ();

app.Run();