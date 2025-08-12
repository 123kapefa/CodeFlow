using System.Net.Http.Json;

using ApiGateway.Application.Extensions;

using Ardalis.Result;

using Contracts.DTOs.AnswerService;
using Contracts.DTOs.CommentService;
using Contracts.DTOs.QuestionService;

using Microsoft.AspNetCore.Http;

namespace ApiGateway.Application.Clients;

public sealed class AnswerApi {

  private readonly HttpClient _http;
  private readonly IHttpContextAccessor _ctx;

  public AnswerApi (HttpClient http, IHttpContextAccessor ctx) {
    _http = http;
    _ctx = ctx;
  }

  public Task<List<AnswerDto>?> GetByQuestionAsync (Guid questionId, CancellationToken ct) {
    HeaderPropagation.CopyAuthAndTrace (_http, _ctx.HttpContext!);
    return _http.GetFromJsonAsync<List<AnswerDto>> ($"/answers/question/{questionId}", ct);
  }

  public async Task<IEnumerable<AnswerDto>> GetAnswersByUserIdAsync (Guid userId, CancellationToken ct) {
    HeaderPropagation.CopyAuthAndTrace (_http, _ctx.HttpContext!);
    var response = _http.GetFromJsonAsync<PagedResult<IEnumerable<AnswerDto>>> (
      $"/answers/user/{userId}?page=1&pageSize=5&orderBy=CreatedAt&sortDirection=Ascending", ct);
    return response.Result!.Value;
  }

}