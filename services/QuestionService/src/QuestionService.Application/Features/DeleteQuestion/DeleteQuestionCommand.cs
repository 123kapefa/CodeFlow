using Abstractions.Commands;

namespace QuestionService.Application.Features.DeleteQuestion;

public record DeleteQuestionCommand (Guid QuestionId) : ICommand;
