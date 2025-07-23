using QuestionService.Domain.Filters;

using Abstractions.Commands;

namespace QuestionService.Application.Features.GetQuestions;

public record GetQuestionsCommand(PageParams PageParams, SortParams SortParams) : ICommand;
