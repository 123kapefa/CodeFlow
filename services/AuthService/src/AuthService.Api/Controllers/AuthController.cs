using Ardalis.Result;
using Ardalis.Result.AspNetCore;

using AuthService.Application.Features.LoginUser;
using AuthService.Application.Features.LogoutUser;
using AuthService.Application.Features.RegisterUser;
using AuthService.Application.Reauests;
using AuthService.Application.Requests;
using AuthService.Application.Response;
using AuthService.Application.Responses;

using Contracts.Commands;

using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[TranslateResultToActionResult]
public class AuthController : ControllerBase {

  [HttpPost ("register")]
  public async Task<Result<Guid>> RegisterUser (
    [FromBody] RegisterUserRequest request, 
    [FromServices] ICommandHandler<Guid, RegisterUserCommand> handler) =>
    await handler.Handle (
      new RegisterUserCommand (request.Username, request.Email, request.Password), 
      new CancellationToken (false));
  
  
  [HttpPost("login")]
  public async Task<Result<LoginResponse>> LoginUser (
    [FromBody] LoginUserRequest request, 
    [FromServices] ICommandHandler<LoginResponse, LoginUserCommand> handler) =>
    await handler.Handle (
      new LoginUserCommand (request.Email, request.Password), 
      new CancellationToken(false));

  [HttpGet ("logout")] 
  public async Task<Result<bool>> LogoutUser (
    [FromBody] string token,
    [FromServices] ICommandHandler<bool, LogoutUserCommand>  handler) =>
  await handler.Handle(
    new LogoutUserCommand(token),
    new CancellationToken(false));
    
}