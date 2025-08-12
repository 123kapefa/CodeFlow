using AnswerService.Application.Features.CreateAnswer;
using AnswerService.Application.Features.DeleteAnswer;
using AnswerService.Application.Features.EditAnswer;
using AnswerService.Application.Features.GetAnswersByQuestionId;
using AnswerService.Application.Features.GetAnswersByUserId;

using Abstractions.Commands;

using AnswerService.Application.Features.DeleteAnswersByUserId;
using AnswerService.Application.Features.UpdateAnswerAccept;
using AnswerService.Domain.Repositories;
using AnswerService.Infrastructure.Repositories;

using Ardalis.Result;

using Contracts.DTOs.AnswerService;
using Contracts.Responses.AnswerService;

using FluentValidation;
using AnswerService.Application.Features.UpdateAnswerVote;
using AnswerService.Application.Features.GetAnswerHistory;
using Contracts.DTOs.AnswerService;

namespace AnswerService.Api.Extensions;

public static class HandlerCollectionExtensions {

  public static WebApplicationBuilder AddHandlers (this WebApplicationBuilder builder) {
    
    builder.Services.AddScoped<IAnswerRepository, AnswerRepository> ();
    builder.Services.AddScoped<IAnswerChangingHistoryRepository, AnswerChangingHistoryRepository> ();
    
    builder.Services.AddScoped<ICommandHandler<CreateAnswerResponse, CreateAnswerCommand>, CreateAnswerHandler> ();
    builder.Services.AddScoped<ICommandHandler<EditAnswerCommand>, EditAnswerHandler> ();
    builder.Services.AddScoped<ICommandHandler<DeleteAnswerCommand>, DeleteAnswerHandler> ();
    builder.Services.AddScoped<ICommandHandler<DeleteAnswersByUserIdCommand>, DeleteAnswersByUserIdHandler> ();
    builder.Services.AddScoped<ICommandHandler<UpdateAnswerAcceptCommand>, UpdateAnswerAcceptHandler> ();
    builder.Services.AddScoped<ICommandHandler<PagedResult<IEnumerable<AnswerDto>>, GetAnswersByUserIdCommand>, GetAnswersByUserIdHandler> ();
    builder.Services.AddScoped<ICommandHandler<IEnumerable<AnswerDto>, GetAnswersByQuestionIdCommand>, GetAnswersByQuestionIdHandler> ();
    
    builder.Services.AddScoped<IValidator<CreateAnswerCommand>, CreateAnswerValidator> ();
    builder.Services.AddScoped<IValidator<EditAnswerCommand>, EditAnswerValidator> ();
      
        return builder;
  }

}