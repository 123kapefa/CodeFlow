using CommentService.Application.DTO;
using Contracts.Commands;

namespace CommentService.Application.Features.CreateComment;

public record CreateCommentCommand( CreateCommentDTO CreateCommentDTO ) : ICommand;
