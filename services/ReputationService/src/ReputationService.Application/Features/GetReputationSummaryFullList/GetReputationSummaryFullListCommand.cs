using Abstractions.Commands;

using Contracts.Common.Filters;

namespace ReputationService.Application.Features.GetReputationSummaryFullList;

public record GetReputationSummaryFullListCommand (Guid UserId, PageParams PageParams, SortParams SortParams) : ICommand;