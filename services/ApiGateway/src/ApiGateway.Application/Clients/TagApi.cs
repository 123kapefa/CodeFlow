using System.Net.Http.Json;
using System.Text.Json;

using ApiGateway.Application.Extensions;

using Ardalis.Result;

using Contracts.DTOs.TagService;
using Contracts.Requests.TagService;
using Contracts.Responses.TagService;

using Microsoft.AspNetCore.Http;

namespace ApiGateway.Application.Clients;

public sealed class TagApi
{
  private readonly HttpClient _http; private readonly IHttpContextAccessor _ctx;
  public TagApi(HttpClient http, IHttpContextAccessor ctx) { _http = http; _ctx = ctx; }

  public Task<TagDTO?> GetAsync(int id, CancellationToken ct) {
    HeaderPropagation.CopyAuthAndTrace(_http, _ctx.HttpContext!);
    return _http.GetFromJsonAsync<TagDTO>($"/tags/{id}", ct);
  }

  // батч — сильно уменьшит N запросов
  public async Task<IEnumerable<TagDTO>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken ct)
  {
    HeaderPropagation.CopyAuthAndTrace(_http, _ctx.HttpContext!);
    var response = await _http.PostAsJsonAsync("/tags/by-ids", new GetTagsByIdsRequest(ids), ct);
    response.EnsureSuccessStatusCode();
    var result = await response.Content.ReadFromJsonAsync<IEnumerable<TagDTO>>(cancellationToken: ct);
    
    return result ?? new List<TagDTO>();
  }

  // ensure: вернуть существующие + создать недостающие и вернуть их Id
  public async Task<EnsureTagsResponse> EnsureAsync(EnsureTagsRequest req, CancellationToken ct)
  {
    HeaderPropagation.CopyAuthAndTrace(_http, _ctx.HttpContext!);
    var resp = await _http.PostAsJsonAsync("/tags/create-tags", req, ct);
    resp.EnsureSuccessStatusCode();
    return (await resp.Content.ReadFromJsonAsync<EnsureTagsResponse>(cancellationToken: ct))!;
  }
  
  public async Task<IEnumerable<ParticipationDTO>> GetTagsByUserIdAsync (Guid userId, CancellationToken ct) {
    HeaderPropagation.CopyAuthAndTrace (_http, _ctx.HttpContext!);
    var response = _http.GetFromJsonAsync<PagedResult<IEnumerable<ParticipationDTO>>> (
      $"/tags/user/{userId}/participation?page=1&pageSize=5&orderBy=QuestionsCreated&sortDirection=Ascending", 
      ct);
    
    Console.WriteLine(JsonSerializer.Serialize(response));
    
    return response.Result!.Value;
  }


  public async Task<IEnumerable<WatchedTagDTO>> GetWatchedByUserIdAsync (Guid userId, CancellationToken ct) {
    HeaderPropagation.CopyAuthAndTrace (_http, _ctx.HttpContext!);
    var response = _http.GetFromJsonAsync<IEnumerable<WatchedTagDTO>> ($"/users/{userId}/watched-tags", ct);
    return response.Result! ?? new List<WatchedTagDTO> ();
  }

}