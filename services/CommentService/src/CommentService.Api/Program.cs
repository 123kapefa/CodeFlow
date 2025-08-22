using CommentService.Api.Extensions;
using CommentService.Infrastructure.Data;
using Contracts.Bootstrap;
using Messaging.Extensions;
using Microsoft.EntityFrameworkCore;


EnvBootstrapper.Load();


var builder = WebApplication.CreateBuilder(args);

builder.AddBase ();
builder.AddCustomSwagger ();
builder.AddDatabase ();
builder.AddCustomSerilog ();
builder.AddHandlers ();
builder.AddCommentMessaging ();
builder.Services.AddMessaging ();

builder.Services.AddControllers();

var app = builder.Build();

app.UseCustomSwagger ();
app.UseBase ();

using(var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;
    try {
        var context = services.GetRequiredService<CommentServiceDbContext>();
        context.Database.Migrate();
    }
    catch(Exception ex) {
        Console.WriteLine($"������ ��� ���������� ��������: {ex.Message}");
        throw;
    }
}

app.MapControllers ();

app.Run();