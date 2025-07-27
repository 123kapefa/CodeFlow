using AnswerService.Api.Extensions;
using Messaging.Extensions;

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

app.Run ();