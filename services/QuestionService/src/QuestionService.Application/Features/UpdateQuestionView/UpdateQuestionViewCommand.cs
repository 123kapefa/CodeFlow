using Abstractions.Commands;

namespace QuestionService.Application.Features.UpdateQuestionView;

public record UpdateQuestionViewCommand (Guid QuestionId) : ICommand;