using Abstractions.Commands;

using Contracts.Common.Filters;

namespace QuestionService.Application.Features.GetQuestionsByIds;

public record GetQuestionsByIdsCommand (IEnumerable<Guid> QuestionIds)
  : ICommand;