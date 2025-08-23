using Abstractions.Commands;

namespace QuestionService.Application.Features.UpdateQuestionAccept;

public record UpdateQuestionAcceptCommand( Guid QuestionId, Guid? OldAcceptedAnswerId, Guid AcceptedAnswerId, Guid AnswerUserId ) : ICommand;
