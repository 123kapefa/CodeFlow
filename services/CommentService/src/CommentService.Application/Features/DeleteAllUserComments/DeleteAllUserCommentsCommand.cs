using Abstractions.Commands;

namespace CommentService.Application.Features.DeleteAllUserComments;

public record DeleteAllUserCommentsCommand (Guid UserId) : ICommand;
