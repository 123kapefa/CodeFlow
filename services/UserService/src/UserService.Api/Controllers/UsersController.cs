using Abstractions.Commands;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;

using Contracts.Common.Filters;
using Contracts.DTOs.UserService;
using Contracts.Requests.UserService;
using Contracts.Responses.UserService;

using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using UserService.Application.Features.DeleteUser;
using UserService.Application.Features.GetUserFullInfo;
using UserService.Application.Features.GetUsers;
using UserService.Application.Features.UpdateUserInfo;
using UserService.Application.Features.UpdateUserProfile;

using PageParams = UserService.Domain.Filters.PageParams;
using SortParams = UserService.Domain.Filters.SortParams;

namespace UserService.Api.Controllers;

[ApiController]
[Route ("users")]
[TranslateResultToActionResult]
public class UsersController : ControllerBase {    


    [HttpGet]
    [SwaggerOperation(
    Summary = "Получить список пользователей.",
    Description = "Получает список пользователей для указанной страницы.",
    OperationId = "User_Get")]
    public async Task<Result<PagedResult<IEnumerable<UserShortDTO>>>> GetUsersAsync(
        [FromQuery] PageParams pageParams,
        [FromQuery] SortParams sortParams,
        [FromQuery] SearchFilter searchFilter,    
        [FromServices] ICommandHandler<PagedResult<IEnumerable<UserShortDTO>>, GetUsersCommand> handler ) => 
        await handler.Handle(new GetUsersCommand(pageParams,sortParams, searchFilter), new CancellationToken(false));


    [HttpGet("{userId}")]
    [SwaggerOperation(
    Summary = "Получить полную информацию о пользователе по userID.",
    Description = "Получает информацию о пользователе(возвращает обьект UserFullInfoDTO).",
    OperationId = "User_Get")]
    public async Task<Result<UserFullInfoDTO>> GetUserFullInfoAsync(
        Guid userId,
        [FromServices] ICommandHandler<UserFullInfoDTO, GetUserFullInfoCommand> handler ) =>
        await handler.Handle(new GetUserFullInfoCommand(userId), new CancellationToken(false));
    
    
    [HttpPut("user")]
    [SwaggerOperation(
    Summary = "Обновить пользователя.",
    Description = "Обновляет информацию о пользователе.",
    OperationId = "User_Put")]
    public async Task<Result<UpdateUserProfileResponse>> UpdateUserInfoProfileAsync(
        [FromForm] UpdateUserProfileRequest request,
        [FromServices] ICommandHandler<UpdateUserProfileResponse, UpdateUserProfileCommand> handler ) =>
        await handler.Handle(new UpdateUserProfileCommand(request), new CancellationToken(false));
    
    
    [HttpPut("info")]
    [SwaggerOperation(
    Summary = "Обновить пользователя.",
    Description = "Обновляет информацию о пользователе.",
    OperationId = "User_Put")]
    public async Task<Result> UpdateUserInfoAsync(
      [FromBody] UserInfoUpdateDTO userDto,
      [FromServices] ICommandHandler<UpdateUserInfoCommand> handler ) =>
      await handler.Handle(new UpdateUserInfoCommand(userDto), new CancellationToken(false));


    [HttpDelete("{userId}")]
    [SwaggerOperation(
    Summary = "Удалить пользователя.",
    Description = "Удаляет запись из таблиц UserIfo и UserStatistic(каскадно).",
    OperationId = "User_Delete")]
    public async Task<Result> DeleteUserInfoAsync(
        Guid userId,
        [FromServices] ICommandHandler<DeleteUserCommand> handler ) =>
        await handler.Handle(new DeleteUserCommand(userId), new CancellationToken(false));

}