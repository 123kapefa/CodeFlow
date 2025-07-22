using CommentService.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddBase ();
builder.UseCustomSwagger ();
builder.AddDatabase ();
builder.AddCustomSerilog ();
builder.AddHandlers ();

builder.Services.AddControllers();

var app = builder.Build();

app.UseCustomSwagger ();
app.UseBase ();
app.MapControllers ();

app.Run();