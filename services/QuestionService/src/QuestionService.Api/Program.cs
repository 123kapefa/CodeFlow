using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using QuestionService.Infrastructure.Data;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<QuestionServiceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Main")));

// Swagger
builder.Services.AddEndpointsApiExplorer ();
builder.Services.AddSwaggerGen (options => {
    options.SwaggerDoc ("v1", new OpenApiInfo {
        Title = "Product API",
        Version = "v1",
        Description = "Пример документации Swagger для QuestionService"
    });
});

builder.Services.AddControllers();

WebApplication app = builder.Build();

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
app.UseDeveloperExceptionPage();

app.MapControllers ();


app.Run();