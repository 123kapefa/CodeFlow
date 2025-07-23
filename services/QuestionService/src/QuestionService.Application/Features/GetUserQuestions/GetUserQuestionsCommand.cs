using QuestionService.Domain.Filters;

using Abstractions.Commands;

namespace QuestionService.Application.Features.GetUserQuestions;
public record GetUserQuestionsCommand( Guid UserId, PageParams PageParams, SortParams SortParams )  : ICommand;
