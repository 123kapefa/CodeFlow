using AuthService.Infrastructure;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder (args);

builder.Services.AddControllers ();



builder.Services.AddOpenApi ();
builder.Services.AddScoped<AuthServiceDbContext> (_ =>
  new AuthServiceDbContext (builder.Configuration.GetConnectionString (nameof(AuthServiceDbContext))!));

// Swagger
builder.Services.AddEndpointsApiExplorer ();
builder.Services.AddSwaggerGen (options => {
    options.SwaggerDoc ("v1", new OpenApiInfo {
        Title = "Product API",
        Version = "v1",
        Description = "Пример документации Swagger для AuthService"
    });
});

var app = builder.Build ();

//TODO подумать как разрулить это (docker стартует в Production, а swagger запускается из Development)

//if (app.Environment.IsDevelopment ()) {
//    app.UseSwagger ();
//    app.UseSwaggerUI (options => {
//        options.SwaggerEndpoint ("/swagger/v1/swagger.json", "Product API v1");
//    });
//}

app.UseSwagger ();
app.UseSwaggerUI (options => {
    options.SwaggerEndpoint ("/swagger/v1/swagger.json", "Product API v1");
});

//app.UseHttpsRedirection ();

app.UseAuthorization ();

app.MapControllers ();

app.Run ();