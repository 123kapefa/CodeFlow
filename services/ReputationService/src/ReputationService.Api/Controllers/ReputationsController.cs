using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Abstractions.Commands;

using Ardalis.Result;
using Ardalis.Result.AspNetCore;

using Contracts.Common.Filters;
using Contracts.DTOs.ReputationService;

using Microsoft.AspNetCore.Mvc;

using ReputationService.Application.Features.GetReputationMonthList;
using ReputationService.Application.Features.GetReputationSummaryFullList;
using ReputationService.Application.Features.GetReputationSummaryShortList;
using ReputationService.Domain.Entities;

namespace ReputationService.Api.Controllers;

[ApiController]
[Route ("[controller]")]
[TranslateResultToActionResult]
public class ReputationsController : ControllerBase {

  [HttpGet ("reputation-month-list/{userId:guid}")]
  public async Task<Result<IReadOnlyList<ReputationShortDto>>> GetReputationSummaryList (
    [FromRoute] Guid userId,
    [FromQuery] PageParams pageParams,
    [FromQuery] SortParams sortParams,
    [FromServices] ICommandHandler<IReadOnlyList<ReputationShortDto>, GetReputationMonthListCommand> handler) =>
    await handler.Handle (new GetReputationMonthListCommand (userId, pageParams, sortParams),
      new CancellationToken (false));
  
  [HttpGet ("reputation-summary-sort-list/{userId:guid}")]
  public async Task<Result<PagedResult<IReadOnlyList<ReputationShortDto>>>> GetReputationSummaryShortList (
    [FromRoute] Guid userId,
    [FromQuery] PageParams pageParams,
    [FromQuery] SortParams sortParams,
    [FromServices] ICommandHandler<PagedResult<IReadOnlyList<ReputationShortDto>>, GetReputationSummaryShortListCommand> handler)
  => await handler.Handle (new GetReputationSummaryShortListCommand (userId, pageParams, sortParams),
    new CancellationToken (false));

  [HttpGet ("reputation-summary-full-list/{userId:guid}")]
  public async Task<Result<PagedResult<IReadOnlyList<ReputationGroupedByDayDto>>>> GetReputationSummaryFullList (
    [FromRoute] Guid userId,
    [FromQuery] PageParams pageParams,
    [FromQuery] SortParams sortParams,
    [FromServices] ICommandHandler<PagedResult<IReadOnlyList<ReputationGroupedByDayDto>>, GetReputationSummaryFullListCommand> handler)
    => await handler.Handle (new GetReputationSummaryFullListCommand (userId, pageParams, sortParams),
      new CancellationToken (false));
  
}