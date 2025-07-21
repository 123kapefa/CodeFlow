using Abstractions.Commands;

namespace QuestionService.Application.Features.GetQuestionShort;
public record GetQuestionShortCommand(Guid questionId) : ICommand;
