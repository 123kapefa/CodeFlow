using Abstractions.Commands;

namespace QuestionService.Application.Features.UpdateQuestionAnswers;
public record UpdateQuestionAnswersCommand(Guid QuestionId) : ICommand;
