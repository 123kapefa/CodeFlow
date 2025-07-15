using Contracts.Commands;

namespace CommentService.Application.Features.DeleteCommentById;

public record DeleteCommentByIdCommand(Guid CommentId) : ICommand;
