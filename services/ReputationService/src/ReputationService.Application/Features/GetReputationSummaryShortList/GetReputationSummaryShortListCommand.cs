using Abstractions.Commands;

using Contracts.Common.Filters;

namespace ReputationService.Application.Features.GetReputationSummaryShortList;

public record GetReputationSummaryShortListCommand (Guid UserId, PageParams PageParams, SortParams SortParams) : ICommand;