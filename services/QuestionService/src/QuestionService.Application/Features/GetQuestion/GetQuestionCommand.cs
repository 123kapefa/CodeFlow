using Abstractions.Commands;

namespace QuestionService.Application.Features.GetQuestion;

public record GetQuestionCommand(Guid questionId) :ICommand;