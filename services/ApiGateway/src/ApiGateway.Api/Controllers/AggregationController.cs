using System.Text.Json;

using ApiGateway.Application.Extensions;
using ApiGateway.Application.Services;

using Contracts.Common.Filters;

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

  [HttpPost ("get-question")]
  public async Task<IActionResult> AggregateQuestionWithAnswersAndComments ([FromBody] QuestionRequest request) {
    if (string.IsNullOrEmpty (request.QuestionId)) {
      return BadRequest ("ID вопроса не указан");
    }

    var results = new Dictionary<string, object> ();
    var resultLock = new object ();

    var questionTask = _httpService.FetchDataAsync ("question", $"api/questions/{request.QuestionId}", "GET", null
      , results, resultLock);

    var answersTask = _httpService.FetchDataAsync ("answers", $"api/answers/question/{request.QuestionId}", "GET", null
      , results, resultLock);

    var questionCommentsTask = _httpService.FetchDataAsync ("questionComments"
      , $"api/comments/question/{request.QuestionId}", "GET", null, results, resultLock);

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

            var task = _httpService.FetchDataAsync (commentKey, $"api/comments/answer/{answerId}", "GET", null
              , answerComments, resultLock);

            answerCommentTasks.Add (task);
          }
        }

        await Task.WhenAll (answerCommentTasks);
        results["answerComments"] = answerComments;
      }
      else {
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

            var task = _httpService.FetchDataAsync (tagKey, $"api/tags/{tagId}", "GET", null, questionTags, resultLock);

            questionTagTasks.Add (task);
          }
        }

        await Task.WhenAll (questionTagTasks);
        results["tags"] = questionTags;
      }
      else {
        Console.WriteLine ("Не удалось найти массив questionTags в объекте вопроса");
      }
    }

    return Ok (results);
  }

  [HttpPost ("get-questions")]
  public async Task<IActionResult> AggregateQuestionsWithTags (
    [FromQuery] PageParams pageParams
    , [FromQuery] SortParams sortParams) {
    var results = new Dictionary<string, object> ();
    var resultLock = new object ();

    var questionTask = _httpService.FetchDataAsync ("questions"
      , $"api/questions?{pageParams.ToQueryString ()}&{sortParams.ToQueryString ()}", "GET", null, results, resultLock);

    await questionTask;

    Console.WriteLine (JsonSerializer.Serialize (results));

    if (results.ContainsKey ("questions") && results["questions"] is JsonElement questionRoot) {
      if (questionRoot.TryGetProperty ("value", out var questionsElement) &&
          questionsElement.ValueKind == JsonValueKind.Array) {
        var tagTasks = new List<Task> ();
        var tagsResult = new Dictionary<string, object> ();

        foreach (var question in questionsElement.EnumerateArray ()) {
          if (question.TryGetProperty ("questionTags", out var tagsElement) &&
              tagsElement.ValueKind == JsonValueKind.Array) {
            foreach (var tagRef in tagsElement.EnumerateArray ()) {
              if (tagRef.TryGetProperty ("tagId", out var idElement)) {
                int tagId = idElement.GetInt32 ();
                string tagKey = $"tag-{tagId}";

                Console.WriteLine ($"Запрос для тега: tagKey = {tagKey}, tagId = {tagId}");

                var tagTask = _httpService.FetchDataAsync (
                  tagKey, $"api/tags/{tagId}", 
                  "GET", 
                  null, 
                  tagsResult, resultLock);

                tagTasks.Add (tagTask);
              }
            }
          }
          else {
            Console.WriteLine ("Свойство 'questionTags' отсутствует или не является массивом.");
          }
        }

        await Task.WhenAll (tagTasks);


        await Task.WhenAll (tagTasks);

        results["tags"] = tagsResult;
      }
      else {
        Console.WriteLine ("Вопросы не найдены или их структура неверна.");
      }
    }
    else {
      Console.WriteLine ("Не удалось получить список вопросов.");
    }

    return Ok (results);
  }

}