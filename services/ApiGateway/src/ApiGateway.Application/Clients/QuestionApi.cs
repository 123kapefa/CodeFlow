using System.Net.Http.Json;
using System.Web;

using ApiGateway.Application.Extensions;

using Ardalis.Result;

using Contracts.Common.Filters;
using Contracts.DTOs.QuestionService;
using Contracts.Requests.ApiGateway;
using Contracts.Responses.QuestionService;

using Microsoft.AspNetCore.Http;

namespace ApiGateway.Application.Clients;

public sealed class QuestionApi {

    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _ctx;

    public QuestionApi( HttpClient http, IHttpContextAccessor ctx ) {
        _http = http;
        _ctx = ctx;
    }

    public Task<QuestionDTO?> GetAsync( Guid questionId, CancellationToken ct ) {
        HeaderPropagation.CopyAuthAndTrace(_http, _ctx.HttpContext!);
        var relativePath = $"/questions/{questionId}";
        var baseUrl = _http.BaseAddress?.ToString() ?? "(base address не установлена)";

        // Формируем конечный URL
        var finalUrl = new Uri(new Uri(baseUrl), relativePath);

        Console.WriteLine($"[DEBUG] Полный путь: {finalUrl}");

        HeaderPropagation.CopyAuthAndTrace(_http, _ctx.HttpContext!);
        return _http.GetFromJsonAsync<QuestionDTO>($"/questions/{questionId}", ct);
    }

    public async Task<Result<CreatedQuestionResponse>> CreateAsync( CreateQuestionRequest req, CancellationToken ct ) {
        HeaderPropagation.CopyAuthAndTrace(_http, _ctx.HttpContext!);
        var response = _http.PostAsJsonAsync("/questions", req, ct);
        response.Result.EnsureSuccessStatusCode();
        return (await response.Result.Content.ReadFromJsonAsync<CreatedQuestionResponse>(cancellationToken: ct))!;
    }

    public async Task<PagedResult<IEnumerable<QuestionShortDTO>>> GetListAsync( string query, CancellationToken ct ) {
        HeaderPropagation.CopyAuthAndTrace(_http, _ctx.HttpContext!);
        var response = await _http.GetFromJsonAsync<PagedResult<IEnumerable<QuestionShortDTO>>>($"/questions?{query}", ct);

        return response ?? new PagedResult<IEnumerable<QuestionShortDTO>>(
          new PagedInfo(1, 30, 0, 0), new List<QuestionShortDTO>());
    }

    public async Task<PagedResult<IEnumerable<QuestionShortDTO>>> GetQuestionSummaryListAsync( Guid userId, string query, CancellationToken ct ) {
        HeaderPropagation.CopyAuthAndTrace(_http, _ctx.HttpContext!);
        var response = await _http.GetFromJsonAsync<PagedResult<IEnumerable<QuestionShortDTO>>>($"/questions/user/{userId}?{query}", ct);

        return response ?? new PagedResult<IEnumerable<QuestionShortDTO>>(
          new PagedInfo(1, 30, 0, 0), new List<QuestionShortDTO>());
    }

    public async Task<IEnumerable<QuestionShortDTO>> GetQuestionsByUserIdAsync(
      Guid userId,
      CancellationToken ct ) {
        HeaderPropagation.CopyAuthAndTrace(_http, _ctx.HttpContext!);
        var response = _http.GetFromJsonAsync<IEnumerable<QuestionShortDTO>>(
          $"/questions/user/{userId}", ct);

        return response.Result ?? new List<QuestionShortDTO>();
    }

    public async Task<IEnumerable<QuestionShortDTO>> GetQuestionsByIdsAsync(
      IEnumerable<Guid> questionIds,
      CancellationToken ct ) {
        HeaderPropagation.CopyAuthAndTrace(_http, _ctx.HttpContext!);

        var response = _http.PostAsJsonAsync($"/questions/get-questions-by-ids", questionIds, ct);
        response.Result.EnsureSuccessStatusCode();
        var questions =
          response.Result.Content.ReadFromJsonAsync<IEnumerable<QuestionShortDTO>>(cancellationToken: ct);

        return questions.Result ?? new List<QuestionShortDTO>();
    }
    
    public async Task<IEnumerable<QuestionTitleDto>> GetQuestionTitlesByIdsAsync(
      IEnumerable<Guid> questionIds,
      CancellationToken ct ) {
      HeaderPropagation.CopyAuthAndTrace(_http, _ctx.HttpContext!);

      var response = _http.PostAsJsonAsync($"/questions/get-questions-title-by-ids", questionIds, ct);
      response.Result.EnsureSuccessStatusCode();
      var questions =
        response.Result.Content.ReadFromJsonAsync<IEnumerable<QuestionTitleDto>> (cancellationToken: ct);

      return questions.Result ?? new List<QuestionTitleDto>();
    }

    public async Task<PagedResult<IEnumerable<QuestionShortDTO>>> GetQuestionsPageByIdsAsync(
      List<Guid> questionIds,
      PageParams pageParams,
      SortParams sortParams,
      CancellationToken ct ) {
        HeaderPropagation.CopyAuthAndTrace(_http, _ctx.HttpContext!);

        var response = _http.PostAsJsonAsync($"/questions/get-questions-by-ids", questionIds, ct);
        response.Result.EnsureSuccessStatusCode();
        var questions =
          response.Result.Content.ReadFromJsonAsync<PagedResult<IEnumerable<QuestionShortDTO>>>(cancellationToken: ct);

        return questions.Result ??
          new PagedResult<IEnumerable<QuestionShortDTO>>(new PagedInfo(1, 5, 0, 0), new List<QuestionShortDTO>());
    }

    public async Task<IEnumerable<QuestionShortDTO>> GetByTagsSortedAsync(
      IEnumerable<int> tagIds,
      string query,
      CancellationToken ct ) {
        HeaderPropagation.CopyAuthAndTrace(_http, _ctx.HttpContext!);
        var response = _http.PostAsJsonAsync($"/questions/get-questions-by-tags?{query}", tagIds, ct);
        response.Result.EnsureSuccessStatusCode();

        var questions =
          await response.Result.Content.ReadFromJsonAsync<IEnumerable<QuestionShortDTO>>(cancellationToken: ct);

        return questions ?? new List<QuestionShortDTO>();

    }

}