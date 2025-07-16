using Contracts.Commands;

namespace CommentService.Application.Features.UpdateComment;

public record UpdateCommentCommand(Guid CommentId, string Content) : ICommand; // TODO или DTO ????
