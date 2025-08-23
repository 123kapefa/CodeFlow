using System;

using Messaging.Extensions;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using ReputationService.Api.Extensions;
using ReputationService.Infrastructure;

var builder = WebApplication.CreateBuilder (args);

builder.AddBase ();
builder.AddHandlers ();
builder.AddDatabase ();
builder.AddCustomSerilog ();
builder.AddCustomSwagger ();
builder.AddReputationMessaging ();
builder.Services.AddMessaging();

var app = builder.Build ();

app.UseCustomSwagger ();
app.UseBase ();
app.MapControllers ();

using(var scope = app.Services.CreateScope()) {
  var services = scope.ServiceProvider;
  try {
    var context = services.GetRequiredService<ReputationServiceDbContext>();
    context.Database.Migrate();
  }
  catch(Exception ex) {
    Console.WriteLine($"Errors: {ex.Message}");
    throw;
  }
}

app.Run ();