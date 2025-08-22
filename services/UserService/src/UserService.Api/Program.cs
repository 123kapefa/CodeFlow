using Contracts.Bootstrap;
using Microsoft.EntityFrameworkCore;
using UserService.Api.Extensions;
using UserService.Application.Services;
using UserService.Infrastructure.Data;
using UserService.Infrastructure.Services;


EnvBootstrapper.Load();

var builder = WebApplication.CreateBuilder(args);

builder.AddBase ();
builder.AddCustomSwagger ();
builder.AddDatabase ();
builder.AddCustomSerilog ();
builder.AddHandlers ();
builder.AddUserMessaging ();
builder.AddCloudStorage ();
builder.Services.AddControllers();

builder.Services.AddScoped<IAvatarStorageService, AvatarStorageService> ();

var app = builder.Build();

app.UseCustomSwagger ();
app.UseBase ();

using(var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;
    try {
        var context = services.GetRequiredService<UserServiceDbContext>();
        context.Database.Migrate();
    }
    catch(Exception ex) {
        Console.WriteLine($"{ex.Message}");
        throw;
    }
}

app.MapControllers ();

app.Run();