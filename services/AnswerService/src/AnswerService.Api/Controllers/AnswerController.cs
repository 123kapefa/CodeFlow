using AnswerService.Application.Features.AcceptAnswer;
using AnswerService.Application.Features.CreateAnswer;
using AnswerService.Application.Features.DeleteAnswer;
using AnswerService.Application.Features.EditAnswer;
using AnswerService.Application.Features.GetAnswersByQuestionId;
using AnswerService.Application.Features.GetAnswersByUserId;
using AnswerService.Application.Requests;
using AnswerService.Application.Responses;
using AnswerService.Domain.Entities;

using Ardalis.Result;
using Ardalis.Result.AspNetCore;

using Contracts.Commands;

using Microsoft.AspNetCore.Mvc;

namespace AnswerService.Api.Controllers;

[ApiController]
[Route ("api/[controller]")]
[TranslateResultToActionResult]
public class AnswerController : ControllerBase {

  [HttpPost]
  public async Task<Result<CreateAnswerResponse>> CreateAnswer (
    [FromBody] CreateAnswerRequest request
    , [FromServices] ICommandHandler<CreateAnswerResponse, CreateAnswerCommand> handler) =>
    await handler.Handle (new CreateAnswerCommand (request), new CancellationToken (false));

  [HttpPost ("edit/{answerId:guid}")]
  public async Task<Result> EditAnswer (
    [FromRoute] Guid answerId
    , [FromBody] UpdateAnswerRequest request
    , [FromServices] ICommandHandler<EditAnswerCommand> handler) =>
    await handler.Handle (new EditAnswerCommand(answerId, request), new CancellationToken (false));

  [HttpDelete ("{answerId:guid}")]
  public async Task<Result> DeleteAnswer (
    [FromRoute] Guid answerId,
    [FromServices] ICommandHandler<DeleteAnswerCommand> handler) =>
  await handler.Handle (new DeleteAnswerCommand(answerId), new CancellationToken (false));

  [HttpGet ("question/{questionId:guid}")]
  public async Task<GetAnswersResponse> GetAnswersByQuestionId (
    [FromRoute] Guid questionId
    , [FromServices] ICommandHandler<GetAnswersResponse, GetAnswersByQuestionIdCommand> handler) =>
    await handler.Handle (new GetAnswersByQuestionIdCommand(questionId), new CancellationToken (false));

  [HttpGet ("user/{userId:guid}")]
  public async Task<GetAnswersResponse> GetAnswersByUserId (
    [FromRoute] Guid userId
    , [FromServices] ICommandHandler<GetAnswersResponse, GetAnswersByUserIdCommand> handler) =>
    await handler.Handle (new GetAnswersByUserIdCommand(userId), new CancellationToken (false));
  
  [HttpGet ("{answerId:guid}/question/{questionId:guid}")]
  public async Task<Result> AcceptAnswer (
    [FromRoute] Guid answerId
    , [FromRoute] Guid questionId
    , [FromServices] ICommandHandler<AcceptAnswerCommand> handler) =>
  await handler.Handle (new AcceptAnswerCommand(answerId, questionId), new CancellationToken (false));
  
}