using Abstractions.Commands;

using CommentService.Application.Features.CreateComment;
using CommentService.Application.Features.DeleteComment;
using CommentService.Application.Features.GetCommentById;
using CommentService.Application.Features.GetComments;
using CommentService.Application.Features.UpdateComment;
using CommentService.Domain.Repositories;
using CommentService.Infrastructure.Repositories;

using Contracts.CommentService.DTOs;

namespace CommentService.Api.Extensions;

public static class HandlerCollectionExtensions {

  public static WebApplicationBuilder AddHandlers (this WebApplicationBuilder builder) {

    builder.Services.AddScoped<ICommentRepository, CommentRepository>();

    builder.Services.AddScoped<ICommandHandler<CreateCommentCommand>, CreateCommentHandler>();
    builder.Services.AddScoped<ICommandHandler<CommentDTO, GetCommentByIdCommand>, GetCommentByIdHandler>();
    builder.Services.AddScoped<ICommandHandler<DeleteCommentCommand>, DeleteCommentHandler>();
    builder.Services.AddScoped<ICommandHandler<UpdateCommentCommand>, UpdateCommentHandler>();
    builder.Services.AddScoped<ICommandHandler<IEnumerable<CommentDTO>, GetCommentsCommand>, GetCommentsHandler>();

    return builder;
  }

}