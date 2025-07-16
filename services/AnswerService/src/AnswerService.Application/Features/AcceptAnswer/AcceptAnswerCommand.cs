using Contracts.Commands;

namespace AnswerService.Application.Features.AcceptAnswer;

public record AcceptAnswerCommand (Guid Id, Guid QuestionId) : ICommand;