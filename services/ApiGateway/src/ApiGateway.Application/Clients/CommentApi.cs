using System.Net.Http.Json;

using ApiGateway.Application.Extensions;

using Contracts.DTOs.CommentService;

using Microsoft.AspNetCore.Http;

namespace ApiGateway.Application.Clients;

public class CommentApi {

  private readonly HttpClient _http;
  private readonly IHttpContextAccessor _ctx;

  public CommentApi (HttpClient http, IHttpContextAccessor ctx) {
    _http = http;
    _ctx = ctx;
  }

  public Task<IEnumerable<CommentDTO>?> GetQuestionCommentsAsync(Guid questionId, CancellationToken ct)
  {
    HeaderPropagation.CopyAuthAndTrace(_http, _ctx.HttpContext!);
    return _http.GetFromJsonAsync<IEnumerable<CommentDTO>>($"/comments/question/{questionId}", ct);
  }
  
  public async Task<Dictionary<Guid, List<CommentDTO>>?> GetCommentsForAnswersAsync(IEnumerable<Guid> answerIds, CancellationToken ct)
  {
    HeaderPropagation.CopyAuthAndTrace(_http, _ctx.HttpContext!);
    var response = await _http.PostAsJsonAsync("/comments/get-comments-by-answers", answerIds, ct);
    response.EnsureSuccessStatusCode();
    var result = await response.Content.ReadFromJsonAsync<IEnumerable<CommentDTO>>(cancellationToken: ct);
    
    if (result is null) return new Dictionary<Guid, List<CommentDTO>>();
    
    var grouped = result
     .GroupBy(comment => comment.TargetId)
     .ToDictionary(
        group => group.Key,
        group => group.ToList()
      );
    
    return grouped;
  }
  

}