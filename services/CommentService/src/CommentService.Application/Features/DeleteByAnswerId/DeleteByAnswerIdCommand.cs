using Abstractions.Commands;

namespace CommentService.Application.Features.DeleteByAnswerId;

public record DeleteByAnswerIdCommand( Guid AnswerId ) : ICommand;
