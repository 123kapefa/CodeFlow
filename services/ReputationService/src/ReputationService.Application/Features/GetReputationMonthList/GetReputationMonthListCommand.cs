using Abstractions.Commands;

using Contracts.Common.Filters;

namespace ReputationService.Application.Features.GetReputationMonthList;

public record GetReputationMonthListCommand (
  Guid UserId,
  PageParams PageParams,
  SortParams SortParams) : ICommand;