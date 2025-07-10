using Contracts.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using QuestionService.Application.DTO;
using QuestionService.Application.Features.GetQuestion;
using QuestionService.Application.Features.GetQuestionHistory;
using QuestionService.Application.Features.GetQuestionShort;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Repositories;
using QuestionService.Infrastructure.Data;
using QuestionService.Infrastructure.Repositories;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<QuestionServiceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Main")));

builder.Services.AddScoped<IQuestionServiceRepository, QuestionServiceRepository>();

builder.Services.AddScoped<ICommandHandler<QuestionDTO, GetQuestionCommand>, GetQuestionHandler>();
builder.Services.AddScoped<ICommandHandler<QuestionShortDTO, GetQuestionShortCommand>, GetQuestionShortHandler>();
builder.Services.AddScoped<ICommandHandler<IEnumerable<QuestionHistoryDTO>, GetQuestionHistoryCommand>, GetQuestionHistoryHandler>();


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