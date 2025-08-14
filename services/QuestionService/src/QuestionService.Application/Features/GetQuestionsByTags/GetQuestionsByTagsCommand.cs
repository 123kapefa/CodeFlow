using Abstractions.Commands;

using Contracts.Common.Filters;

namespace QuestionService.Application.Features.GetQuestionsByTags;

public record GetQuestionsByTagsCommand (IEnumerable<int> TagIds, PageParams PageParams, SortParams SortParams)
  : ICommand;