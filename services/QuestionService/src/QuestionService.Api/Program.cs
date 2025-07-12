using Ardalis.Result;
using Contracts.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using QuestionService.Application.DTO;
using QuestionService.Application.Features.CreateQuestion;
using QuestionService.Application.Features.DeleteQuestion;
using QuestionService.Application.Features.GetQuestion;
using QuestionService.Application.Features.GetQuestionHistory;
using QuestionService.Application.Features.GetQuestions;
using QuestionService.Application.Features.GetQuestionShort;
using QuestionService.Application.Features.GetQuestionTags;
using QuestionService.Application.Features.GetUserQuestions;
using QuestionService.Application.Features.UpdateQuestion;
using QuestionService.Application.Features.UpdateQuestionAccept;
using QuestionService.Application.Features.UpdateQuestionAnswers;
using QuestionService.Application.Features.UpdateQuestionView;
using QuestionService.Application.Features.UpdateQuestionVote;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Repositories;
using QuestionService.Infrastructure.Data;
using QuestionService.Infrastructure.Repositories;
using System.Collections.Generic;
using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<QuestionServiceDbContext>(options => {
    options.UseNpgsql(builder.Configuration.GetConnectionString("Main"))
     .EnableSensitiveDataLogging()                   
           .LogTo(Console.WriteLine, LogLevel.Information);
});

builder.Services.AddScoped<IQuestionServiceRepository, QuestionServiceRepository>();
builder.Services.AddScoped<IValidator<CreateQuestionCommand>, CreateQuestionValidator>();
builder.Services.AddScoped<IValidator<UpdateQuestionCommand>, UpdateQuestionValidator>();

builder.Services.AddScoped<ICommandHandler<QuestionDTO, GetQuestionCommand>, GetQuestionHandler>();
builder.Services.AddScoped<ICommandHandler<QuestionShortDTO, GetQuestionShortCommand>, GetQuestionShortHandler>();
builder.Services.AddScoped<ICommandHandler<IEnumerable<QuestionHistoryDTO>, GetQuestionHistoryCommand>, GetQuestionHistoryHandler>();
builder.Services.AddScoped<ICommandHandler<IEnumerable<QuestionTagDTO>, GetQuestionTagsCommand>, GetQuestionTagsHandler>();
builder.Services.AddScoped<ICommandHandler<CreateQuestionCommand>, CreateQuestionHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateQuestionCommand>, UpdateQuestionHandler>();
builder.Services.AddScoped<ICommandHandler<DeleteQuestionCommand>, DeleteQuestionHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateQuestionAcceptCommand>, UpdateQuestionAcceptHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateQuestionViewCommand>, UpdateQuestionViewHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateQuestionVoteCommand>, UpdateQuestionVoteHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateQuestionAnswersCommand>, UpdateQuestionAnswersHandler>();
builder.Services.AddScoped<ICommandHandler<PagedResult<IEnumerable<QuestionShortDTO>>, GetQuestionsCommand>, GetQuestionsHandler>();
builder.Services.AddScoped<ICommandHandler<PagedResult<IEnumerable<QuestionShortDTO>>, GetUserQuestionsCommand>, GetUserQuestionsHandler>();


// Swagger
builder.Services.AddEndpointsApiExplorer ();
builder.Services.AddSwaggerGen (options => {   
          
    options.SwaggerDoc ("v1", new OpenApiInfo {
        Title = "Product API",
        Version = "v1",
        Description = "Пример документации Swagger для QuestionService"
    });

    options.EnableAnnotations();
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