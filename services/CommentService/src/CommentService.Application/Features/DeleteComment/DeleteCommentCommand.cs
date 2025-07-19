using Contracts.Commands;

namespace CommentService.Application.Features.DeleteCommentById;

public record DeleteCommentCommand(Guid CommentId) : ICommand;
