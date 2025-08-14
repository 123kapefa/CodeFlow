using Abstractions.Commands;

using Contracts.Common.Filters;

namespace AnswerService.Application.Features.GetAnswerQuestionIdsByUserId;

public record GetAnswerQuestionIdsByUserIdCommand (Guid UserId, PageParams PageParams, SortParams SortParams)
  : ICommand;