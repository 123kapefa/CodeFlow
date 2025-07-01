using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Contracts.Commands;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTO;
using UserService.Application.Features.CreateUserInfo;
using UserService.Application.Features.DeleteUser;
using UserService.Application.Features.GetUserFullInfo;
using UserService.Application.Features.GetUsers;
using UserService.Application.Features.UpdateUserInfo;
using UserService.Application.Features.UpdateUserReputation;
using UserService.Application.Features.UpdateUserVisit;
using UserService.Domain.Entities;
using UserService.Domain.Filters;

namespace UserService.Api.Controllers;

[ApiController]
[Route ("users")]
[TranslateResultToActionResult]
public class UsersController : ControllerBase {

    private readonly IUserInfoService _userInfoService;
    private readonly int _pageSize; // ???? ПОМЕНЯТЬ или ПОЛУЧАТЬ ОТ UI ????

    [HttpGet]
    public async Task<Result<PagedResult<IEnumerable<UserShortDTO>>>> GetUsersAsync(
        [FromQuery] PageParams pageParams,
        [FromQuery] SortParams sortParams,
        [FromServices] ICommandHandler<PagedResult<IEnumerable<UserShortDTO>>, GetUsersCommand> handler ) => 
        await handler.Handle(new GetUsersCommand(pageParams,sortParams), new CancellationToken(false));

    [HttpGet ("{pageNumber:int}")]
    public async Task<IActionResult> GetUsersByRatingAsync (int pageNumber) {
        if (pageNumber <= 0)
            return BadRequest (new { message = "Page number must be greater than zero." });

    [HttpGet("{userId}")]
    public async Task<Result<UserFullInfoDTO>> GetUserFullInfoAsync(
        Guid userId,
        [FromServices] ICommandHandler<Result<UserFullInfoDTO>, GetUserFullInfoCommand> handler ) =>
        await handler.Handle(new GetUserFullInfoCommand(userId), new CancellationToken(false));

        return Ok (users);
    }

    [HttpPost("create/{userId}/{userName}")] //TODO нужен для проверки
    public async Task<Result> CreateUserInfoAsync(
        Guid userId, 
        string userName,
        [FromServices]ICommandHandler<CreateUserInfoCommand> handler) =>
        await handler.Handle(new CreateUserInfoCommand(userId, userName), new CancellationToken(false));


    [HttpPut("info")]
    public async Task<Result> UpdateUserInfoAsync( 
        [FromBody]UserInfoUpdateDTO userDto, 
        [FromServices] ICommandHandler<UpdateUserInfoCommand>  handler) => 
        await handler.Handle(new UpdateUserInfoCommand(userDto), new CancellationToken(false));

        try {
            IEnumerable<UserShortDTO> users = await _userInfoService.GetUsersByDateAsync (pageNumber, _pageSize);
            return Ok (users);
        }
        catch (Exception ex) {
            return StatusCode (500, new { message = "Internal server error.", details = ex.Message });
        }

        
    }

    [HttpPost("info")]
    public async Task<IActionResult> CreateUserInfoAsync ([FromBody] UserInfoCreateDTO userDto) {
        try {
            bool created = await _userInfoService.CreateUserInfoAsync (userDto);
            return created ? Ok (created) : Conflict (new { message = "User already exists." });
        }
        catch (ArgumentException ex) {
            return BadRequest (new { message = ex.Message });
        }
        catch (Exception ex) {
            return StatusCode (500, new { message = "Internal server error.", details = ex.Message });
        }
    }

    [HttpPut("reputation/{userId}/{reputation}")] //TODO нужен для проверки
    public async Task<Result> UpdateUserReputation(
        Guid userId,
        int reputation,
        [FromServices] ICommandHandler<UpdateUserReputationCommand> handler ) =>
        await handler.Handle(new UpdateUserReputationCommand(userId, reputation), new CancellationToken(false));

    [HttpPut("visit/{userId}")] //TODO нужен для проверки
    public async Task<Result> UpdateUserVisitAsync(
        Guid userId,
        [FromServices] ICommandHandler<UpdateUserVisitCommand> handler ) =>
        await handler.Handle(new UpdateUserVisitCommand(userId), new CancellationToken(false));

    [HttpPut("statistic")]
    public async Task<IActionResult> UpdateUserStatisticAsync ([FromBody]UserStatisticUpdateDto userDto) {
        try {
            bool updated = await _userInfoService.UpdateUserStatisticAsync (userDto);
            return updated ? Ok (updated) : NotFound ();
        }
        catch (ArgumentException ex) {
            return BadRequest (new { message = ex.Message });
        }
        catch (Exception ex) {
            return StatusCode (500, new { message = "Internal server error.", details = ex.Message });
        }
    }

    [HttpDelete("{userId}")]
    public async Task<Result> DeleteUserInfoAsync(
        Guid userId,
        [FromServices] ICommandHandler<DeleteUserCommand> handler ) =>
        await handler.Handle(new DeleteUserCommand(userId), new CancellationToken(false));

}
