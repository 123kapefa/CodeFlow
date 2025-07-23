using AuthService.Api.Extensions;

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
app.UseAuth ();
app.MapControllers ();

app.Run ();