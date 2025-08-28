using System.Text.Json;

using ApiGateway.Application.Clients;
using ApiGateway.Application.Extensions;
using ApiGateway.Application.Services;

using Ardalis.Result;

using Contracts.Common.Filters;
using Contracts.DTOs.AnswerService;
using Contracts.DTOs.CommentService;
using Contracts.DTOs.QuestionService;
using Contracts.DTOs.TagService;
using Contracts.DTOs.UserService;

using Microsoft.AspNetCore.Mvc;

using Contracts.Requests.ApiGateway;
using Contracts.Requests.TagService;

using Microsoft.AspNetCore.Authorization;

namespace ApiGateway.Api.Controllers;

[ApiController]
[Route("api/aggregate")]
public class AggregationController : ControllerBase {

    private readonly HttpService _httpService;

    private readonly UserApi _users;
    private readonly QuestionApi _questions;
    private readonly AnswerApi _answers;
    private readonly TagApi _tags;
    private readonly CommentApi _comments;
    private readonly ReputationApi _reputations;

    public AggregationController(
      HttpService httpService,
      QuestionApi questions,
      AnswerApi answers,
      TagApi tags,
      CommentApi comments,
      UserApi users,
      ReputationApi reputations) {
        _httpService = httpService;
        _questions = questions;
        _answers = answers;
        _tags = tags;
        _comments = comments;
        _users = users;
        _reputations = reputations;
    }

    [HttpPost("get-question")]
    public async Task<IActionResult> GetQuestion( [FromBody] QuestionRequest request, CancellationToken ct ) {
        if(!Guid.TryParse(request.QuestionId, out var qid))
            return BadRequest("Invalid questionId");

        var questionTask = _questions.GetAsync(qid, ct);
        var answersTask = _answers.GetByQuestionAsync(qid, ct);
        var questionCommentsTask = _comments.GetQuestionCommentsAsync(qid, ct);

        await Task.WhenAll(questionTask, answersTask, questionCommentsTask);

        var question = await questionTask;
        var answers = await answersTask ?? new List<AnswerDto>();
        var questionComments = await questionCommentsTask ?? new List<CommentDTO>();

        if(question is null) return NotFound("Question not found");

        var answerComments = await _comments.GetCommentsForAnswersAsync(answers.Select(a => a.Id), ct);

        var tagIds = question.QuestionTags?.Select(t => t.TagId).Distinct().ToList() ?? new();
        var tags = await _tags.GetByIdsAsync(tagIds, ct);

        var userIds = new List<Guid>();

        userIds.Add(question.UserId);
        userIds.AddRange(answers.Select(a => a.UserId));
        userIds.AddRange(questionComments.Select(c => c.AuthorId));
        if(answerComments != null) {
            foreach(var kvp in answerComments) {
                var comments = kvp.Value;
                userIds.AddRange(comments.Select(c => c.AuthorId));
            }
        }

        userIds = userIds.Distinct().ToList();

        var users = await _users.GetUsersByIdsAsync(userIds, ct);

        var result = new {
            question,
            questionComments,
            answers,
            answerComments,
            tags,
            users
        };
        return Ok(result);
    }

    [HttpPost("create-question")]
    public async Task<IActionResult> CreateQuestion( [FromBody] CreateQuestionRequest request, CancellationToken ct ) {
        var createdTags = await _tags.EnsureAsync(
          new EnsureTagsRequest(request.QuestionDto.NewTags.Select(t => t.Name).ToList()), ct);

        request.QuestionDto.NewTags = createdTags.TagIds.Select(t => new CreateTagDto { Id = t }).ToList();

        var createdQuestion = await _questions.CreateAsync(request, ct);
        return Ok(createdQuestion);
    }

    [HttpPost("get-questions")]
    public async Task<IActionResult> AggregateQuestionsWithTags(
      [FromQuery] PageParams pageParams,
      [FromQuery] SortParams sortParams,
      [FromQuery] TagFilter tagFilter,
      CancellationToken ct ) {
        var questionsList = await _questions.GetListAsync(
          $"{pageParams.ToQueryString()}&{sortParams.ToQueryString()}&{tagFilter.ToQueryString()}", ct);

        var userIds = questionsList.Value.Select(q => q.UserId).ToList();
        var tagIds = questionsList.Value.Select(q => q.QuestionTags.Select(t => t.TagId)).SelectMany(t => t).Distinct()
         .ToList();

        var tagsListTask = _tags.GetByIdsAsync(tagIds, ct);
        var usersListTask = _users.GetUsersByIdsAsync(userIds, ct);

        await Task.WhenAll(tagsListTask, usersListTask);

        var tagsList = await tagsListTask;
        var usersList = await usersListTask;

        var result = new { questionsList, tagsList, usersList, };

        return Ok(result);
    }
        
    [HttpGet("get-user-summary/{userId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> AggregateUserSummary( [FromRoute] Guid userId, CancellationToken ct ) {
        if(userId == Guid.Empty)
            return BadRequest("UserId не указан.");

        var userTask = _users.GetUserFullInfoAsync(userId, ct);
        var questionsUserListTask = _questions.GetQuestionsByUserIdAsync(userId, ct);
        var answersUserListTask = _answers.GetAnswersByUserIdAsync(userId, ct);
        var tagsUserListTask = _tags.GetTagsByUserIdAsync(userId, ct);

        await Task.WhenAll(userTask, questionsUserListTask, answersUserListTask, tagsUserListTask);

        var user = await userTask;
        var questionsUserList = await questionsUserListTask;
        var answersUserList = await answersUserListTask;
        var tagsUserList = await tagsUserListTask;

        var questionIds = answersUserList.Select(a => a.QuestionId).ToList();
        var questionsAnswerList = await _questions.GetQuestionsByIdsAsync(questionIds, ct);

        var result = new {
            user,
            questionsUserList,
            questionsAnswerList,
            tagsUserList,
        };

        return Ok(result);
    }

    [Authorize]
    [HttpGet("recommended/{userId:guid}")]
    public async Task<IActionResult> GetRecommended(
      [FromRoute] Guid userId,
      [FromQuery] PageParams pageParams,
      [FromQuery] SortParams sortParams,
      CancellationToken ct = default ) {

        var watched = await _tags.GetWatchedByUserIdAsync(userId, ct);
        var watchedTagIds = watched?.Select(t => t.TagId).Distinct().ToList() ?? new List<int>();

        if(watchedTagIds.Count == 0) {

            var questionsList = await _questions.GetListAsync(
              $"{pageParams.ToQueryString()}&{sortParams.ToQueryString()}", ct);

            var qList = questionsList.Value.ToList();
            var userIds = qList
             .Select(q => q.UserId)
             .Distinct()
             .ToList();
            var tagIds = qList
             .SelectMany(q => q.QuestionTags.Select(t => t.TagId))
             .Distinct()
             .ToList();

            var usersTask = _users.GetUsersByIdsAsync(userIds, ct);
            var tagsTask = _tags.GetByIdsAsync(tagIds, ct);
            await Task.WhenAll(usersTask, tagsTask);

            return Ok(new {
                items =
                qList.Select(q => new { question = q, matchedTagIds = Array.Empty<int>(), matchCount = 0, score = 0d }),
                users = await usersTask,
                tags = await tagsTask
            });
        }

        var candidates = await _questions.GetByTagsSortedAsync(
          watchedTagIds,
          $"{pageParams.ToQueryString()}&{sortParams.ToQueryString()}",
          ct);

        var now = DateTime.UtcNow;
        var items = candidates.Select(q =>
        {
            var matched = q.QuestionTags?.Select(t => t.TagId).Distinct().Where(watchedTagIds.Contains).ToArray() ??
              Array.Empty<int>();

            var ageHours = (now - q.CreatedAt).TotalHours;
            var freshness = Math.Exp(-ageHours / 72.0);
            var score = matched.Length * 3 + freshness;

            return new { question = q, matchedTagIds = matched, matchCount = matched.Length, score };
        }).ToList();

        var userIdsRec = items.Select(x => x.question.UserId).Distinct().ToList();
        var tagIdsRec = items.SelectMany(x => x.question.QuestionTags.Select(t => t.TagId)).Distinct().ToList();
        var usersTaskRec = _users.GetUsersByIdsAsync(userIdsRec, ct);
        var tagsTaskRec = _tags.GetByIdsAsync(tagIdsRec, ct);
        await Task.WhenAll(usersTaskRec, tagsTaskRec);

        return Ok(new { items, users = await usersTaskRec, tags = await tagsTaskRec });
    }

    [HttpPost("get-questions-summary/{userId:guid}")]
    public async Task<IActionResult> AggregateQuestionsSummary(
      [FromRoute] Guid userId,
      [FromQuery] PageParams pageParams,
      [FromQuery] SortParams sortParams,
      CancellationToken ct ) {
        var questionsList = await _questions.GetQuestionSummaryListAsync(
          userId,
          $"{pageParams.ToQueryString()}&{sortParams.ToQueryString()}",
          ct);

        var tagIds = questionsList.Value.Select(q => q.QuestionTags.Select(t => t.TagId))
         .SelectMany(t => t)
         .Distinct()
         .ToList();

        var tagsListTask = _tags.GetByIdsAsync(tagIds, ct);

        await Task.WhenAll(tagsListTask);

        var tagsList = await tagsListTask;

        var result = new { questionsList, tagsList, };

        return Ok(result);
    }

    [HttpPost("get-answers-summary/{userId:guid}")]
    public async Task<IActionResult> AggregateAnswersSummary(
      [FromRoute] Guid userId,
      [FromQuery] PageParams pageParams,
      [FromQuery] SortParams sortParams,
      CancellationToken ct ) {

        var questionIds = await _answers.GetAnswerQuestionIdsByUserIdAsync(
          userId,
          $"{pageParams.ToQueryString()}&{sortParams.ToQueryString()}",
          ct);
      

        if (!questionIds.Any ()) {
          return Ok (new {
            questionsList =
              new PagedResult<IEnumerable<QuestionShortDTO>> (
                new PagedInfo ((int)pageParams.Page!, (int)pageParams.PageSize!, 0, 0), new List<QuestionShortDTO> ()),
            tagsList = new List<TagDTO> (),
            usersList = new List<UserForQuestionDto> ()
          });
        }

        var questionsList = await _questions.GetQuestionsByIdsAsync(
          questionIds, ct);

        var userIds = questionsList.Select(q => q.UserId).ToList();
        var tagIds = questionsList.Select(q => q.QuestionTags.Select(t => t.TagId))
         .SelectMany(t => t)
         .Distinct()
         .ToList();

        var tagsListTask = _tags.GetByIdsAsync(tagIds, ct);
        var usersListTask = _users.GetUsersByIdsAsync(userIds, ct);

        await Task.WhenAll(tagsListTask, usersListTask);

        var tagsList = await tagsListTask;
        var usersList = await usersListTask;

        var result = new { questionsList, tagsList, usersList, };

        return Ok(result);
    }

    [HttpGet ("get-reputation-full-list/{userId:guid}")]
    public async Task<IActionResult> AggregateReputationSummary ([FromRoute] Guid userId, CancellationToken ct) {
      var reputationList = await _reputations.GetReputationFullListByUserIdAsync(userId, ct);

      var questionIds = reputationList.Value
       .SelectMany(r => r.Events?.Select(e => e.ParentId) ?? Enumerable.Empty<Guid>())
       .Where(id => id != Guid.Empty)
       .Distinct()
       .ToList();

      var questionTitles = await _questions.GetQuestionTitlesByIdsAsync(questionIds, ct);
      var titleById = (questionTitles ?? Enumerable.Empty<QuestionTitleDto>())
       .GroupBy(q => q.QuestionId)                // на случай дубликатов
       .Select(g => g.First())
       .ToDictionary(q => q.QuestionId, q => q.Title);

      foreach (var dayGroup in reputationList.Value) {
        if (dayGroup?.Events == null) continue;

        foreach (var ev in dayGroup.Events) {
          if (ev.ParentId != Guid.Empty && titleById.TryGetValue(ev.ParentId, out var title)) {
            ev.Title = title;
          }
        }
      }

      return Ok(new { reputationList });


    }
}

