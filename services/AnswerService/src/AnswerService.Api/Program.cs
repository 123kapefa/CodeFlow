using AnswerService.Api.Extensions;

using AnswerService.Infrastructure;

using Messaging.Extensions;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder (args);

builder.AddBase ();
builder.AddHandlers ();
builder.AddDatabase ();
builder.AddCustomSerilog ();
builder.AddCustomSwagger ();
builder.AddAnswerMessaging();
builder.Services.AddMessaging();

var app = builder.Build ();

app.UseCustomSwagger ();
app.UseBase ();
app.MapControllers ();

using(var scope = app.Services.CreateScope()) {
  var services = scope.ServiceProvider;
  try {
    var context = services.GetRequiredService<AnswerServiceDbContext>();
    context.Database.Migrate();
  }
  catch(Exception ex) {
    Console.WriteLine($"������ ��� ���������� ��������: {ex.Message}");
    throw;
  }
}

app.Run ();