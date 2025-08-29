using System.Net.Http.Json;

using ApiGateway.Application.Extensions;

using Ardalis.Result;

using Contracts.DTOs.QuestionService;
using Contracts.DTOs.ReputationService;

using Microsoft.AspNetCore.Http;

namespace ApiGateway.Application.Clients;

public class ReputationApi {

  private readonly HttpClient _http;
  private readonly IHttpContextAccessor _ctx;

  public ReputationApi (HttpClient http, IHttpContextAccessor ctx) {
    _http = http;
    _ctx = ctx;
  }
  
  public async Task<PagedResult<IReadOnlyList<ReputationGroupedByDayDto>>> GetReputationFullListByUserIdAsync(
    Guid userId,
    CancellationToken ct ) {
    HeaderPropagation.CopyAuthAndTrace(_http, _ctx.HttpContext!);
    var response = _http.GetFromJsonAsync<PagedResult<IReadOnlyList<ReputationGroupedByDayDto>>>(
      $"/reputations/reputation-summary-full-list/{userId}?page=1&pageSize=30&sortDirection=Descending", ct);

    return response.Result ??
      new PagedResult<IReadOnlyList<ReputationGroupedByDayDto>>(new PagedInfo(1, 30, 0, 0), new List<ReputationGroupedByDayDto>());
  }

}