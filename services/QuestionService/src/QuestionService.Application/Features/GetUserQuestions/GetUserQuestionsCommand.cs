
using Abstractions.Commands;

using Contracts.Common.Filters;

namespace QuestionService.Application.Features.GetUserQuestions;
public record GetUserQuestionsCommand( Guid UserId, PageParams PageParams, SortParams SortParams )  : ICommand;
