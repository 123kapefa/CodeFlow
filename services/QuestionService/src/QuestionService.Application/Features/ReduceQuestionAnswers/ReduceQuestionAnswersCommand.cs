using Abstractions.Commands;

namespace QuestionService.Application.Features.ReduceQuestionAnswers;

public record ReduceQuestionAnswersCommand(Guid QuestionId) : ICommand;
