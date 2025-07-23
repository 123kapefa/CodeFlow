using AnswerService.Application.Features.AcceptAnswer;
using AnswerService.Application.Features.CreateAnswer;
using AnswerService.Application.Features.DeleteAnswer;
using AnswerService.Application.Features.EditAnswer;
using AnswerService.Application.Features.GetAnswersByQuestionId;
using AnswerService.Application.Features.GetAnswersByUserId;

using Abstractions.Commands;

using AnswerService.Domain.Repositories;
using AnswerService.Infrastructure.Repositories;

using Contracts.AnswerService.Responses;

using FluentValidation;

namespace AnswerService.Api.Extensions;

public static class HandlerCollectionExtensions {

  public static WebApplicationBuilder AddHandlers (this WebApplicationBuilder builder) {
    
    builder.Services.AddScoped<IAnswerRepository, AnswerRepository> ();
    builder.Services.AddScoped<IAnswerChangingHistoryRepository, AnswerChangingHistoryRepository> ();
    
    builder.Services.AddScoped<ICommandHandler<CreateAnswerResponse, CreateAnswerCommand>, CreateAnswerHandler> ();
    builder.Services.AddScoped<ICommandHandler<EditAnswerCommand>, EditAnswerHandler> ();
    builder.Services.AddScoped<ICommandHandler<DeleteAnswerCommand>, DeleteAnswerHandler> ();
    builder.Services.AddScoped<ICommandHandler<AcceptAnswerCommand>, AcceptAnswerHandler> ();
    builder.Services.AddScoped<ICommandHandler<GetAnswersResponse, GetAnswersByUserIdCommand>, GetAnswersByUserIdHandler> ();
    builder.Services.AddScoped<ICommandHandler<GetAnswersResponse, GetAnswersByQuestionIdCommand>, GetAnswersByQuestionIdHandler> ();
    
    builder.Services.AddScoped<IValidator<CreateAnswerCommand>, CreateAnswerValidator> ();
    builder.Services.AddScoped<IValidator<EditAnswerCommand>, EditAnswerValidator> ();
    return builder;
  }

}