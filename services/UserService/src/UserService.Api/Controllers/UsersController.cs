using Abstractions.Commands;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Contracts.UserService.DTOs;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using UserService.Application.Features.CreateUserInfo;
using UserService.Application.Features.DeleteUser;
using UserService.Application.Features.GetUserFullInfo;
using UserService.Application.Features.GetUsers;
using UserService.Application.Features.UpdateUserInfo;
using UserService.Application.Features.UpdateUserReputation;
using UserService.Application.Features.UpdateUserVisit;
using UserService.Domain.Filters;

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
        [FromServices] ICommandHandler<PagedResult<IEnumerable<UserShortDTO>>, GetUsersCommand> handler ) => 
        await handler.Handle(new GetUsersCommand(pageParams,sortParams), new CancellationToken(false));


    [HttpGet("{userId}")]
    [SwaggerOperation(
    Summary = "Получить полную информацию о пользователе по userID.",
    Description = "Получает информацию о пользователе(возвращает обьект UserFullInfoDTO).",
    OperationId = "User_Get")]
    public async Task<Result<UserFullInfoDTO>> GetUserFullInfoAsync(
        Guid userId,
        [FromServices] ICommandHandler<UserFullInfoDTO, GetUserFullInfoCommand> handler ) =>
        await handler.Handle(new GetUserFullInfoCommand(userId), new CancellationToken(false));


    //TODO УДАЛИТЬ
    //[HttpPost("create/{userId}/{userName}")] 
    //[SwaggerOperation(
    //Summary = "Создать пользователя.",
    //Description = "Создает пользователя.",
    //OperationId = "User_Post")]
    //public async Task<Result> CreateUserInfoAsync(
    //    Guid userId, 
    //    string userName,
    //    [FromServices]ICommandHandler<CreateUserInfoCommand> handler) =>
    //    await handler.Handle(new CreateUserInfoCommand(userId, userName), new CancellationToken(false));


    [HttpPut("info")]
    [SwaggerOperation(
    Summary = "Обновить пользователя.",
    Description = "Обновляет информацию о пользователе.",
    OperationId = "User_Put")]
    public async Task<Result> UpdateUserInfoAsync(
      [FromBody] UserInfoUpdateDTO userDto,
      [FromServices] ICommandHandler<UpdateUserInfoCommand> handler ) =>
      await handler.Handle(new UpdateUserInfoCommand(userDto), new CancellationToken(false));

    // TODO УДАЛИТЬ
    //[HttpPut("reputation/{userId}/{reputation}")] 
    //[SwaggerOperation(
    //Summary = "Обновить репутацию пользователя.",
    //Description = "Обновляет репутацию пользователя.",
    //OperationId = "User_Put")]
    //public async Task<Result> UpdateUserReputation(
    //    Guid userId,
    //    int reputation,
    //    [FromServices] ICommandHandler<UpdateUserReputationCommand> handler ) =>
    //    await handler.Handle(new UpdateUserReputationCommand(userId, reputation), new CancellationToken(false));


    // TODO УДАЛИТЬ
    //[HttpPut("visit/{userId}")] 
    //[SwaggerOperation(
    //Summary = "Обновить количество визитов пользователя.",
    //Description = "Обновляет количество визитов пользователя.",
    //OperationId = "User_Put")]
    //public async Task<Result> UpdateUserVisitAsync(
    //    Guid userId,
    //    [FromServices] ICommandHandler<UpdateUserVisitCommand> handler ) =>
    //    await handler.Handle(new UpdateUserVisitCommand(userId), new CancellationToken(false));


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