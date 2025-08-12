using Abstractions.Commands;

namespace CommentService.Application.Features.GetCommentsByAnswerIds;

public record GetCommentsByAnswerIdsCommand (IEnumerable<Guid> AnswerIds) : ICommand;