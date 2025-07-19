using Contracts.Commands;

namespace AnswerService.Application.Features.GetAnswersByQuestionId;

public record GetAnswersByQuestionIdCommand (Guid QuestionId) : ICommand;