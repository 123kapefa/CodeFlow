using Abstractions.Commands;

namespace QuestionService.Application.Features.GetQuestionHistory;

public record GetQuestionHistoryCommand( Guid questionId ) : ICommand;