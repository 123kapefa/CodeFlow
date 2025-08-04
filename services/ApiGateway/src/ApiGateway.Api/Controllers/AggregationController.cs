using System.Text.Json;

using ApiGateway.Application.Clients;
using ApiGateway.Application.Extensions;
using ApiGateway.Application.Services;

using Contracts.Common.Filters;
using Contracts.DTOs.AnswerService;
using Contracts.DTOs.CommentService;
using Contracts.DTOs.QuestionService;
using Contracts.DTOs.TagService;

using Microsoft.AspNetCore.Mvc;

using Contracts.Requests.ApiGateway;
using Contracts.Requests.TagService;

using Microsoft.AspNetCore.Authorization;

namespace ApiGateway.Api.Controllers;

[ApiController]
[Route ("api/aggregate")]
public class AggregationController : ControllerBase {

  private readonly HttpService _httpService;

  private readonly UserApi _users;
  private readonly QuestionApi _questions;
  private readonly AnswerApi _answers;
  private readonly TagApi _tags;
  private readonly CommentApi _comments;

  public AggregationController (
    HttpService httpService,
    QuestionApi questions,
    AnswerApi answers,
    TagApi tags,
    CommentApi comments,
    UserApi users) {
    _httpService = httpService;
    _questions = questions;
    _answers = answers;
    _tags = tags;
    _comments = comments;
    _users = users;
  }

  [HttpPost ("get-question")]
  public async Task<IActionResult> GetQuestion ([FromBody] QuestionRequest request, CancellationToken ct) {
    if (!Guid.TryParse (request.QuestionId, out var qid))
      return BadRequest ("Invalid questionId");

    var questionTask = _questions.GetAsync (qid, ct);
    var answersTask = _answers.GetByQuestionAsync (qid, ct);
    var questionCommentsTask = _comments.GetQuestionCommentsAsync (qid, ct);

    await Task.WhenAll (questionTask, answersTask, questionCommentsTask);

    var question = await questionTask;
    var answers = await answersTask ?? new List<AnswerDto> ();
    var questionComments = await questionCommentsTask ?? new List<CommentDTO> ();

    if (question is null) return NotFound ("Question not found");

    var answerComments = await _comments.GetCommentsForAnswersAsync (answers.Select (a => a.Id), ct);

    var tagIds = question.QuestionTags?.Select (t => t.TagId).Distinct ().ToList () ?? new ();
    var tags = await _tags.GetByIdsAsync (tagIds, ct);

    var userIds = new List<Guid> ();

    userIds.Add (question.UserId);
    userIds.AddRange (answers.Select (a => a.UserId));
    userIds.AddRange (questionComments.Select (c => c.AuthorId));
    if (answerComments != null) {
      foreach (var kvp in answerComments) {
        var comments = kvp.Value;
        userIds.AddRange (comments.Select (c => c.AuthorId));
      }
      }

    userIds = userIds.Distinct ().ToList ();

    var users = await _users.GetUsersByIdsAsync (userIds, ct);

    var result = new {
      question,
      questionComments,
      answers,
      answerComments,
      tags,
      users
    };
    return Ok (result);
          }

  [HttpPost ("create-question")]
  public async Task<IActionResult> CreateQuestion ([FromBody] CreateQuestionRequest request, CancellationToken ct) {
    var createdTags = await _tags.EnsureAsync (
      new EnsureTagsRequest (request.QuestionDto.NewTags.Select (t => t.Name).ToList ()), ct);

    request.QuestionDto.NewTags = createdTags.TagIds.Select (t => new CreateTagDto { Id = t }).ToList ();

    var createdQuestion = await _questions.CreateAsync (request, ct);
    return Ok (createdQuestion);
  }

  [HttpPost ("get-questions")]
  public async Task<IActionResult> AggregateQuestionsWithTags (
    [FromQuery] PageParams pageParams,
    [FromQuery] SortParams sortParams,
    [FromQuery] TagFilter tagFilter,
    CancellationToken ct) {
    var questionsList = await _questions.GetListAsync (
      $"{pageParams.ToQueryString ()}&{sortParams.ToQueryString ()}&{tagFilter.ToQueryString ()}", ct);

    var userIds = questionsList.Value.Select (q => q.UserId).ToList ();
    var tagIds = questionsList.Value.Select (q => q.QuestionTags.Select (t => t.TagId)).SelectMany (t => t).Distinct ()
     .ToList ();

    var tagsListTask = _tags.GetByIdsAsync (tagIds, ct);
    var usersListTask = _users.GetUsersByIdsAsync (userIds, ct);


    await Task.WhenAll (tagsListTask, usersListTask);

    var tagsList = await tagsListTask;
    var usersList = await usersListTask;

    var result = new { questionsList, tagsList, usersList, };

    return Ok (result);
  }

  [HttpGet ("get-user-summary/{userId:guid}")]
  public async Task<IActionResult> AggregateUserSummary ([FromRoute] Guid userId, CancellationToken ct) {
    if (userId == Guid.Empty)
      return BadRequest ("UserId не указан.");

    var userTask = _users.GetUserFullInfoAsync (userId, ct);
    var questionsUserListTask = _questions.GetQuestionsByUserIdAsync (userId, ct);
    var answersUserListTask = _answers.GetAnswersByUserIdAsync (userId, ct);
    var tagsUserListTask = _tags.GetTagsByUserIdAsync (userId, ct);

    await Task.WhenAll (userTask, questionsUserListTask, answersUserListTask, tagsUserListTask);

    var user = await userTask;
    var questionsUserList = await questionsUserListTask;
    var answersUserList = await answersUserListTask;
    var tagsUserList = await tagsUserListTask;

    var questionIds = answersUserList.Select (a => a.QuestionId).ToList ();
    var questionsAnswerList = await _questions.GetQuestionsByIdsAsync (questionIds, ct);

    var result = new {
      user, questionsUserList, questionsAnswerList, tagsUserList,
    };

    return Ok (result);
        }

  [Authorize]
  [HttpGet ("recommended/{userId:guid}")]
  public async Task<IActionResult> GetRecommended (
    [FromRoute] Guid userId,
    [FromQuery] PageParams pageParams,
    [FromQuery] SortParams sortParams,
    CancellationToken ct = default) {

    var watched = await _tags.GetWatchedByUserIdAsync (userId, ct);
    var watchedTagIds = watched?.Select (t => t.TagId).Distinct ().ToList () ?? new List<int> ();

    if (watchedTagIds.Count == 0) {
      
      var questionsList = await _questions.GetListAsync (
        $"{pageParams.ToQueryString ()}&{sortParams.ToQueryString ()}", ct);

      var qList = questionsList.Value.Take ((int)pageParams.PageSize!).ToList ();

      var userIds = qList.Select (q => q.UserId).Distinct ().ToList ();
      var tagIds = qList.SelectMany (q => q.QuestionTags.Select (t => t.TagId)).Distinct ().ToList ();

      var tagsTask = _tags.GetByIdsAsync (tagIds, ct);
      var usersTask = _users.GetUsersByIdsAsync (userIds, ct);
      await Task.WhenAll (tagsTask, usersTask);

      return Ok (new {
        items = qList.Select (q => new { question = q, matchedTagIds = Array.Empty<int> (), score = 0d }),
        tags = await tagsTask,
        users = await usersTask
      });
    }
    
    var candidateTake = Math.Max ((int)pageParams.PageSize! * 4, 40);
    var candidates = new List<QuestionDTO> ();
    // var candidates = await _questions.GetByTagsAsync (watchedTagIds, "any",
    //   (int)pageParams.PageSize!, ct);

    var now = DateTime.UtcNow;

    double Score (QuestionDTO q, IReadOnlyCollection<int> watchedIds) {
      var matchedCount = q.QuestionTags?.Select (t => t.TagId).Distinct ().Count (watchedIds.Contains) ?? 0;

      var ageHours = (now - q.CreatedAt).TotalHours;
      var freshness = Math.Exp (-ageHours / 72.0);

      const double w1 = 3, w3 = 1;
      return w1 * matchedCount + w3 * freshness;
    }

    var ranked = candidates
     .Select (q => new {
        question = q,
        matchedTagIds =
          q.QuestionTags?.Select (t => t.TagId).Distinct ().Where (watchedTagIds.Contains).ToArray () ??
          Array.Empty<int> (),
        score = Score (q, watchedTagIds)
      }).OrderByDescending (x => x.score).ThenByDescending (x => x.question.CreatedAt).Take ((int)pageParams.PageSize).ToList ();

    var userIdsRec = ranked.Select (x => x.question.UserId).Distinct ().ToList ();
    var tagIdsRec = ranked.SelectMany (x => x.question.QuestionTags.Select (t => t.TagId)).Distinct ().ToList ();

    var tagsTaskRec = _tags.GetByIdsAsync (tagIdsRec, ct);
    var usersTaskRec = _users.GetUsersByIdsAsync (userIdsRec, ct);
    await Task.WhenAll (tagsTaskRec, usersTaskRec);

    return Ok (new {
      items = ranked,
      tags = await tagsTaskRec,
      users = await usersTaskRec
    });
  }

}