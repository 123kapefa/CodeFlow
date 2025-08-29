using Abstractions.Commands;

using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using AuthService.Application.Features.EditUser;
using AuthService.Application.Features.EmailChange;
using AuthService.Application.Features.EmailChangeConfirm;
using AuthService.Application.Features.LoginUser;
using AuthService.Application.Features.LogoutUser;
using AuthService.Application.Features.PasswordChange;
using AuthService.Application.Features.PasswordChangeConfirm;
using AuthService.Application.Features.RefreshToken;
using AuthService.Application.Features.RegisterUser;
using AuthService.Application.Features.RemoveUser;

using Contracts.Requests.AuthService;
using Contracts.Responses.AuthService;

using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers;

[ApiController]
[Route ("[controller]")]
[TranslateResultToActionResult]
public class AuthController : ControllerBase {

  [HttpPost ("register")]
  public async Task<Result<Guid>> RegisterUser (
    [FromBody] RegisterUserRequest request
    , [FromServices] ICommandHandler<Guid, RegisterUserCommand> handler) =>
    await handler.Handle (new RegisterUserCommand (request.Username, request.Email, request.Password)
      , new CancellationToken (false));

  [HttpPost ("login")]
  public async Task<Result<LoginResponse>> LoginUser (
    [FromBody] LoginUserRequest request
    , [FromServices] ICommandHandler<LoginResponse, LoginUserCommand> handler) =>
    await handler.Handle (new LoginUserCommand (request.Email, request.Password), new CancellationToken (false));

  [HttpPost ("singin-google")]
    public async Task<Result> LoginUserWithGoogle () => Result.Success();
  
  [HttpPost ("logout")]
  public async Task<Result> LogoutUser (
    [FromBody] string token
    , [FromServices] ICommandHandler<LogoutUserCommand> handler) =>
    await handler.Handle (new LogoutUserCommand (token), new CancellationToken (false));
  
  [HttpPost("refresh-token")]
  public async Task<Result<RefreshTokenResponse>> RefreshToken(
    [FromBody] RefreshTokenRequest request,
    [FromServices] ICommandHandler<RefreshTokenResponse, RefreshTokenCommand> handler) =>
    await handler.Handle(new RefreshTokenCommand(request.RefreshToken), new CancellationToken(false));

  
  [HttpPut("{userId:guid}")]
  public async Task<Result<EditUserDataResponse>> EditUserData (
    [FromRoute] Guid userId
    , [FromBody] EditUserDataRequest request
    , [FromServices] ICommandHandler<EditUserDataResponse, EditUserCommand> handler) =>
    await handler.Handle (new EditUserCommand(userId, request), new CancellationToken (false));
  
  [HttpPost ("email-change/{userId:guid}")]
  public async Task<Result> RequestEmailChange (
    [FromRoute] Guid userId
    , [FromBody] EmailChangeRequest request
    , [FromServices] ICommandHandler<EmailChangeCommand> handler) =>
    await handler.Handle (new EmailChangeCommand (userId, request), new CancellationToken (false));

  [HttpPost ("email-change-confirm/{userId:guid}")]
  public async Task<Result> ConfirmEmailChange (
    [FromRoute] Guid userId
    , [FromBody] EmailChangeConfirmRequest request
    , [FromServices] ICommandHandler<EmailChangeConfirmCommand> handler) =>
    await handler.Handle (new EmailChangeConfirmCommand (userId, request), new CancellationToken (false));

  [HttpPost ("password-change/{userId:guid}")]
  public async Task<Result> PasswordChange (
    [FromRoute] Guid userId
    , [FromBody] PasswordChangeRequest request
    , [FromServices] ICommandHandler<PasswordChangeCommand> handler) =>
    await handler.Handle (new PasswordChangeCommand(userId, request), new CancellationToken (false));
  
  [HttpPost ("password-change-confirm")]
  public async Task<Result> PasswordChangeConfirm (
    [FromBody]PasswordChangeConfirmRequest request
    , [FromServices] ICommandHandler<PasswordChangeConfirmCommand> handler) =>
    await handler.Handle (new PasswordChangeConfirmCommand(request.Email, request.Token), new CancellationToken (false));
  
  [HttpDelete ("{userId:guid}")]
  public async Task<Result> DeleteUser (
    [FromRoute] Guid userId
    , [FromServices] ICommandHandler<RemoveUserCommand> handler) =>
    await handler.Handle (new RemoveUserCommand (userId), new CancellationToken (false));

}