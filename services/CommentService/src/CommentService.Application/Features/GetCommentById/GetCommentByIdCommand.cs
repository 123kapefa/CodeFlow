using Contracts.Commands;

namespace CommentService.Application.Features.GetCommentById;

public record GetCommentByIdCommand(Guid CommentId) : ICommand;
