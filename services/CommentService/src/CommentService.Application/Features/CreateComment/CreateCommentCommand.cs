using Abstractions.Commands;

using Contracts.CommentService.DTOs;

namespace CommentService.Application.Features.CreateComment;

public record CreateCommentCommand( CreateCommentDTO CreateCommentDTO ) : ICommand;
