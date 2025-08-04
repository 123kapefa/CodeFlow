

using Abstractions.Commands;

using Contracts.Common.Filters;

namespace QuestionService.Application.Features.GetQuestions;

public record GetQuestionsCommand(PageParams PageParams, SortParams SortParams) : ICommand;
