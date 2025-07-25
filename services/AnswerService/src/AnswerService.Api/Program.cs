using AnswerService.Api.Extensions;
using AnswerService.Domain.Repositories;
using AnswerService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder (args);

builder.AddBase ();
builder.AddHandlers ();
builder.AddDatabase ();
builder.AddCustomSerilog ();
builder.AddCustomSwagger ();

var app = builder.Build ();

app.UseCustomSwagger ();
app.UseBase ();
app.MapControllers ();

app.Run ();