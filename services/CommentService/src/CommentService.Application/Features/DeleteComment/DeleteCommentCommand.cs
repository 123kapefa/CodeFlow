using Abstractions.Commands;

namespace CommentService.Application.Features.DeleteComment;

public record DeleteCommentCommand(Guid CommentId) : ICommand;
