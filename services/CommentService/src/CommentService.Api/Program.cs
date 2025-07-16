using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using System.Text.Json;
using CommentService.Infrastructure.Data;
using CommentService.Domain.Repositories;
using CommentService.Infrastructure.Repositories;
using Contracts.Commands;
using CommentService.Application.Features.CreateComment;
using CommentService.Domain.Enums;
using CommentService.Application.DTO;
using CommentService.Application.Features.GetCommentById;
using CommentService.Application.Features.DeleteCommentById;
using CommentService.Application.Features.UpdateComment;
using CommentService.Application.Features.GetQuestionComments;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
     .AddJsonOptions(o =>
           o.JsonSerializerOptions.Converters //конвертер для «enum-как-строка».
             .Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase))); ;

builder.Services.AddScoped<ICommentRepository, CommentRepository>();

builder.Services.AddScoped<ICommandHandler<CreateCommentCommand>, CreateCommentHandler>();
builder.Services.AddScoped<ICommandHandler<CommentDTO, GetCommentByIdCommand>, GetCommentByIdHandler>();
builder.Services.AddScoped<ICommandHandler<DeleteCommentCommand>, DeleteCommentHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateCommentCommand>, UpdateCommentHandler>();
builder.Services.AddScoped<ICommandHandler<IEnumerable<CommentDTO>, GetCommentsCommand>, GetCommentsHandler>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1", new OpenApiInfo {
        Title = "Product API",
        Version = "v1",
        Description = "Пример документации Swagger для CommentService"
    });

    options.EnableAnnotations();
});

builder.Services.AddDbContext<CommentDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Main"),
                  npg => npg.MapEnum<TypeTarget>("type_target")));

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options => {
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API v1");
});
app.UseDeveloperExceptionPage();

app.MapControllers();

app.Run();