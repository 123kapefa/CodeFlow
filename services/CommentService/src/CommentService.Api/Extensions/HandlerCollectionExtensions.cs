using Abstractions.Commands;

using CommentService.Application.Features.CreateComment;
using CommentService.Application.Features.DeleteAllUserComments;
using CommentService.Application.Features.DeleteByAnswerId;
using CommentService.Application.Features.DeleteComment;
using CommentService.Application.Features.GetCommentById;
using CommentService.Application.Features.GetComments;
using CommentService.Application.Features.GetCommentsByAnswerIds;
using CommentService.Application.Features.UpdateComment;
using CommentService.Domain.Repositories;
using CommentService.Infrastructure.Repositories;

using Contracts.DTOs.CommentService;

namespace CommentService.Api.Extensions;

public static class HandlerCollectionExtensions {

    public static WebApplicationBuilder AddHandlers( this WebApplicationBuilder builder ) {

        builder.Services.AddScoped<ICommentRepository, CommentRepository>();

        builder.Services.AddScoped<ICommandHandler<CreateCommentCommand>, CreateCommentHandler>();
        builder.Services.AddScoped<ICommandHandler<CommentDTO, GetCommentByIdCommand>, GetCommentByIdHandler>();
        builder.Services.AddScoped<ICommandHandler<IEnumerable<CommentDTO>, GetCommentsByAnswerIdsCommand>, GetCommentsByAnswerIdsHandler>();
        builder.Services.AddScoped<ICommandHandler<DeleteCommentCommand>, DeleteCommentHandler>();
        builder.Services.AddScoped<ICommandHandler<UpdateCommentCommand>, UpdateCommentHandler>();
        builder.Services.AddScoped<ICommandHandler<IEnumerable<CommentDTO>, GetCommentsCommand>, GetCommentsHandler>();
        builder.Services.AddScoped<ICommandHandler<DeleteAllUserCommentsCommand>, DeleteAllUserCommentsHandler>();
        builder.Services.AddScoped<ICommandHandler<DeleteByAnswerIdCommand>, DeleteByAnswerIdHandler>();

        return builder;
    }

}