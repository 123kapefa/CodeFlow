using Auth.Extensions;

using Messaging.Extensions;

using Microsoft.EntityFrameworkCore;

using VoteService.Api.Extensions;
using VoteService.Infrastructure;

var builder = WebApplication.CreateBuilder (args);

builder.AddBase ();
builder.AddHandlers ();
builder.AddDatabase ();
builder.AddCustomSerilog ();
builder.AddCustomSwagger ();
builder.AddVoteMessaging ();
builder.Services.AddJwtAuth (builder.Configuration);
builder.Services.AddMessaging();

var app = builder.Build ();
app.UseJwtAuth ();
app.UseCustomSwagger ();
app.UseBase ();
app.MapControllers ();

using(var scope = app.Services.CreateScope()) {
  var services = scope.ServiceProvider;
  try {
    var context = services.GetRequiredService<VoteServiceDbContext>();
    context.Database.Migrate();
  }
  catch(Exception ex) {
    Console.WriteLine($"������ ��� ���������� ��������: {ex.Message}");
    throw;
  }
}

app.Run ();