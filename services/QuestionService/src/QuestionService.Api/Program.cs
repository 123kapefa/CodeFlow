using Messaging.Extensions;
using Microsoft.EntityFrameworkCore;
using QuestionService.Api.Extensions;
using QuestionService.Infrastructure.Data;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddBase ();
builder.AddCustomSwagger ();
builder.AddDatabase ();
builder.AddCustomSerilog ();
builder.AddHandlers ();
builder.AddQuestionMessaging ();
builder.Services.AddMessaging();

builder.Services.AddControllers();

var app = builder.Build();

app.UseCustomSwagger ();
app.UseBase ();

using(var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;
    try {
        var context = services.GetRequiredService<QuestionServiceDbContext>();
        context.Database.Migrate();
    }
    catch(Exception ex) {
        Console.WriteLine($"������ ��� ���������� ��������: {ex.Message}");
        throw;
    }
}

app.MapControllers ();

app.Run();