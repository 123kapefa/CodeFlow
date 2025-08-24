using System.Text.Json;

using Abstractions.Commands;

using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Contracts.Publishers.VoteService;
using Contracts.Requests.VoteService;
using Contracts.Responses.VoteService;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using VoteService.Application.Handlers.SetVote;
using VoteService.Application.Handlers.Upvote;

namespace VoteService.Api.Controllers;

[ApiController]
[Route ("[controller]")]
[TranslateResultToActionResult]
public class VotesController : ControllerBase {
  
  /// <summary>
  /// Установить голос (лайк/дизлайк).
  /// </summary>
  [HttpPost("set")]
  public async Task<Result<VoteResponse>> SetVote(
    [FromBody] SetVoteRequest request,
    [FromServices] ICommandHandler<VoteResponse, SetVoteCommand> _handler) {
    
    if(User.Identity?.IsAuthenticated is null && User.Identity?.IsAuthenticated == false) {
      return Result<VoteResponse>.Unauthorized();
    }
    
    Guid authorUserId = Guid.Parse (User.FindFirst("sub")?.Value!);

    var command = new SetVoteCommand(
      request.ParentId,
      request.SourceId, 
      request.OwnerUserId, 
      authorUserId, 
      Enum.Parse<VotableSourceType> (request.SourceType, true), 
      Enum.Parse<VoteKind> (request.ValueKind, true)
      );
    
    var result = await _handler.Handle(command, new CancellationToken(false));
    return result;
  }
}