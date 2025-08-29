using Abstractions.Commands;

using Contracts.DTOs.CommentService;

namespace CommentService.Application.Features.CreateComment;

public record CreateCommentCommand( CreateCommentDTO CreateCommentDTO ) : ICommand;
