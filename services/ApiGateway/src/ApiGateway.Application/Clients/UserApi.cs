using System.Net.Http.Json;

using ApiGateway.Application.Extensions;

using Ardalis.Result;

using Contracts.DTOs.AnswerService;
using Contracts.DTOs.UserService;

using Microsoft.AspNetCore.Http;

namespace ApiGateway.Application.Clients;

public class UserApi {

  private readonly HttpClient _http; 
  private readonly IHttpContextAccessor _ctx;

  public UserApi (HttpClient http, IHttpContextAccessor ctx) {
    _http = http; 
    _ctx = ctx;
  }

  public async Task<IEnumerable<UserForQuestionDto>> GetUsersByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct)  {
    HeaderPropagation.CopyAuthAndTrace(_http, _ctx.HttpContext!);
    var response = await _http.PostAsJsonAsync("/users/get-users-by-ids", ids, ct);
    response.EnsureSuccessStatusCode();
    
    var result = await response.Content.ReadFromJsonAsync<IEnumerable<UserForQuestionDto>>(cancellationToken: ct);
    return result ?? new List<UserForQuestionDto>();
  }

  public async Task<UserFullInfoDTO?> GetUserFullInfoAsync (Guid userId, CancellationToken ct) {
    HeaderPropagation.CopyAuthAndTrace(_http, _ctx.HttpContext!);
    var response = await _http.GetFromJsonAsync<UserFullInfoDTO>($"/users/{userId}", ct);
    return response;
  }

}