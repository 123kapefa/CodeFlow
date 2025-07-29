using Abstractions.Commands;

namespace AnswerService.Application.Features.UpdateAnswerAccept;

public record UpdateAnswerAcceptCommand (Guid Id, Guid QuestionId) : ICommand;