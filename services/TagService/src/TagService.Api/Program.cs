using System.Text.Json.Serialization;
using System.Text.Json;

using Abstractions.Commands;

using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using TagService.Infrastructure.Data;
using TagService.Domain.Repositories;
using TagService.Infrastructure.Repositories;
using TagService.Application.Features.Tags.GetTagById;
using FluentValidation;
using TagService.Application.Features.Tags.CreateTag;
using TagService.Application.Features.Tags.UpdateTag;
using TagService.Application.Features.Tags.DeleteTag;
using TagService.Application.Features.Tags.GetTagByName;
using TagService.Application.Features.Tags.UpdateTagRequest;
using TagService.Application.Features.WatchedTags.GetUserWatchedTags;
using TagService.Application.Features.WatchedTags.CreateWatchedTag;
using TagService.Application.Features.WatchedTags.DeleteWatchedTag;
using TagService.Application.Features.WatchedTags.DeleteUserWatchedTags;
using Ardalis.Result;

using Contracts.TagService;

using TagService.Application.Features.Tags.GetTags;
using TagService.Application.Features.Tags.UpdateTagCountQuestion;
using TagService.Application.Features.Tags.UpdateTagWatchers;
using TagService.Application.Features.ParticipationTags.CreateTags;
using TagService.Application.Features.ParticipationTags.UpdateTags;
using TagService.Application.Features.ParticipationTags.DeleteUserTags;
using TagService.Application.Features.ParticipationTags.GetUserTags;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
     .AddJsonOptions(o =>
           o.JsonSerializerOptions.Converters //��������� ��� �enum-���-������.
             .Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)));

builder.Services.AddDbContext<TagServiceDbContext>(options => {
    options.UseNpgsql(builder.Configuration.GetConnectionString("Main"))
     .EnableSensitiveDataLogging()
           .LogTo(Console.WriteLine, LogLevel.Information);
});

builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IWatchedTagRepository, WatchedTagRepository>();
builder.Services.AddScoped<IUserTagParticipationRepository, UserTagParticipationRepository>();

builder.Services.AddScoped<IValidator<CreateTagCommand>, CreateTagValidator>();
builder.Services.AddScoped<IValidator<UpdateTagCommand>, UpdateTagValidator>();

builder.Services.AddScoped<ICommandHandler<TagDTO, GetTagByIdCommand>, GetTagByIdHandler>();
builder.Services.AddScoped<ICommandHandler<TagDTO, GetTagByNameCommand>, GetTagByNameHandler>();
builder.Services.AddScoped<ICommandHandler<PagedResult<IEnumerable<TagDTO>>, GetTagsCommand>, GetTagsHandler>();
builder.Services.AddScoped<ICommandHandler<CreateTagCommand>, CreateTagHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateTagCommand>, UpdateTagHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateTagRequestCommand>, UpdateTagRequestHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateTagCountQuestionCommand>, UpdateTagCountQuestionHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateTagWatchersCommand>, UpdateTagWatchersHandler>();
builder.Services.AddScoped<ICommandHandler<DeleteTagCommand>, DeleteTagHandler>();

builder.Services.AddScoped<ICommandHandler<IEnumerable<WatchedTagDTO>, GetUserWatchedTagsCommand>, GetUserWatchedTagsHandler>();
builder.Services.AddScoped<ICommandHandler<CreateWatchedTagCommand>, CreateWatchedTagHandler>();
builder.Services.AddScoped<ICommandHandler<DeleteWatchedTagCommand>, DeleteWatchedTagHandler>();
builder.Services.AddScoped<ICommandHandler<DeleteUserWatchedTagsCommand>, DeleteUserWatchedTagsHandler>();

builder.Services.AddScoped<ICommandHandler<CreateTagsCommand>, CreateTagsHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateTagsCommand>, UpdateTagsHandler>();
builder.Services.AddScoped<ICommandHandler<DeleteUserTagsCommand>, DeleteUserTagsHandler>();
builder.Services.AddScoped<ICommandHandler<PagedResult<IEnumerable<ParticipationDTO>>, GetUserTagsCommand>, GetUserTagsHandler>();


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1", new OpenApiInfo {
        Title = "Product API",
        Version = "v1",
        Description = "������ ������������ Swagger ��� TagService"
    });

    options.EnableAnnotations();
});


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options => {
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API v1");
});
app.UseDeveloperExceptionPage();

app.MapControllers();

app.Run();