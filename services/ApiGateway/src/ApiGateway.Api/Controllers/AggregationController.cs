using System.Text.Json;

using ApiGateway.Application.Services;

using Microsoft.AspNetCore.Mvc;
using Contracts.Requests.ApiGateway;

namespace ApiGateway.Api.Controllers;

[ApiController]
[Route ("api/aggregate")]
public class AggregationController : ControllerBase {

  private readonly HttpService _httpService;

  public AggregationController (HttpService httpService) {
    _httpService = httpService;
  }

  [HttpPost("get-question")]
  public async Task<IActionResult> AggregateQuestionWithAnswersAndComments ([FromBody] QuestionRequest request) {
    if (string.IsNullOrEmpty (request.QuestionId)) {
      return BadRequest ("ID вопроса не указан");
    }

    var results = new Dictionary<string, object> ();
    var resultLock = new object ();

    var questionTask = _httpService.FetchDataAsync (
      "question", $"api/questions/{request.QuestionId}", "GET", null, results, resultLock);

    var answersTask = _httpService.FetchDataAsync ("answers", $"api/answers/question/{request.QuestionId}", "GET", null, results
      , resultLock);
    
    var questionCommentsTask = _httpService.FetchDataAsync ("questionComments", $"api/comments/question/{request.QuestionId}", "GET"
      , null, results, resultLock);

    await Task.WhenAll (questionTask, answersTask, questionCommentsTask);

    Console.WriteLine (JsonSerializer.Serialize (results));
    
    if (results.ContainsKey ("answers") && results["answers"] is JsonElement answersRoot) {
      if (answersRoot.TryGetProperty ("answers", out var answersElement) &&
          answersElement.ValueKind == JsonValueKind.Array) {
        var answerComments = new Dictionary<string, object> ();
        var answerCommentTasks = new List<Task> ();
        
        foreach (var answer in answersElement.EnumerateArray ()) {
          if (answer.TryGetProperty ("id", out var idElement)) {
            string answerId = idElement.GetString ();
            string commentKey = $"comments-for-answer-{answerId}";
            
            var task = _httpService.FetchDataAsync (commentKey, $"api/comments/answer/{answerId}",
              "GET", null, answerComments, resultLock);

            answerCommentTasks.Add (task);
          }
        }

        await Task.WhenAll (answerCommentTasks);
        results["answerComments"] = answerComments;
      } else {
        Console.WriteLine ("Не удалось найти массив answers в объекте answers");
      }
    }
    
    if (results.ContainsKey ("question") && results["question"] is JsonElement questionElement) {
      if (questionElement.TryGetProperty ("questionTags", out var tagsElement) &&
          tagsElement.ValueKind == JsonValueKind.Array) {
        var questionTags = new Dictionary<string, object> ();
        var questionTagTasks = new List<Task> ();
        
        foreach (var tagRef in tagsElement.EnumerateArray ()) {
          if (tagRef.TryGetProperty ("tagId", out var idElement)) {
            int tagId = idElement.GetInt32 ();
            string tagKey = $"tag-{tagId}";
            
            var task = _httpService.FetchDataAsync (tagKey, $"api/tags/{tagId}",
              "GET", null, questionTags, resultLock);

            questionTagTasks.Add (task);
          }
        }

        await Task.WhenAll (questionTagTasks);
        results["tags"] = questionTags;
      } else {
        Console.WriteLine ("Не удалось найти массив questionTags в объекте вопроса");
      }
    }

    return Ok (results);
  }
  
}


