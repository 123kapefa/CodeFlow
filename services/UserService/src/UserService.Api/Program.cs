using Microsoft.EntityFrameworkCore;
using UserService.Application.Interfaces;
using UserService.Infrastructure.Services;
using UserService.Infrastructure.Data;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddInfrastructure (builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddDbContext<UserServiceDbContext> (options =>
    options.UseNpgsql (builder.Configuration.GetConnectionString ("DefaultConnection")));

builder.Services.AddScoped<IUserInfoService, UserInfoService> ();

// Swagger
builder.Services.AddEndpointsApiExplorer ();
builder.Services.AddSwaggerGen (options => {
    options.SwaggerDoc ("v1", new OpenApiInfo {
        Title = "Product API",
        Version = "v1",
        Description = "������ ������������ Swagger ��� ProductService"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment ()) {
    app.UseSwagger ();
    app.UseSwaggerUI (options => {
        options.SwaggerEndpoint ("/swagger/v1/swagger.json", "Product API v1");
    });
}


app.UseHttpsRedirection(); // ???????Don't need?????
app.MapControllers ();


app.Run();