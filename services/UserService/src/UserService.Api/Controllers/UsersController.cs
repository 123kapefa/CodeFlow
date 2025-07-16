using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Contracts.Commands;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using UserService.Application.DTO;
using UserService.Application.Features.CreateUserInfo;
using UserService.Application.Features.DeleteUser;
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
    OperationId = "Comment_Get")]
    public async Task<Result<PagedResult<IEnumerable<UserShortDTO>>>> GetUsersAsync(
        [FromQuery] PageParams pageParams,
        [FromQuery] SortParams sortParams,
        [FromServices] ICommandHandler<PagedResult<IEnumerable<UserShortDTO>>, GetUsersCommand> handler ) => 
        await handler.Handle(new GetUsersCommand(pageParams,sortParams), new CancellationToken(false));
  

    [HttpPost("create/{userId}/{userName}")] //TODO нужен для проверки
    [SwaggerOperation(
    Summary = "Создать пользователя.",
    Description = "Создает пользователя.",
    OperationId = "Comment_Post")]
    public async Task<Result> CreateUserInfoAsync(
        Guid userId, 
        string userName,
        [FromServices]ICommandHandler<CreateUserInfoCommand> handler) =>
        await handler.Handle(new CreateUserInfoCommand(userId, userName), new CancellationToken(false));


    [HttpPut("info")]
    [SwaggerOperation(
    Summary = "Обновить пользователя.",
    Description = "Обновляет информацию о пользователе.",
    OperationId = "Comment_Put")]
    public async Task<Result> UpdateUserInfoAsync(
      [FromBody] UserInfoUpdateDTO userDto,
      [FromServices] ICommandHandler<UpdateUserInfoCommand> handler ) =>
      await handler.Handle(new UpdateUserInfoCommand(userDto), new CancellationToken(false));


    [HttpPut("reputation/{userId}/{reputation}")] //TODO нужен для проверки
    [SwaggerOperation(
    Summary = "Обновить репутацию пользователя.",
    Description = "Обновляет репутацию пользователя.",
    OperationId = "Comment_Put")]
    public async Task<Result> UpdateUserReputation(
        Guid userId,
        int reputation,
        [FromServices] ICommandHandler<UpdateUserReputationCommand> handler ) =>
        await handler.Handle(new UpdateUserReputationCommand(userId, reputation), new CancellationToken(false));

    [HttpPut("visit/{userId}")] //TODO нужен для проверки
    [SwaggerOperation(
    Summary = "Обновить количество визитов пользователя.",
    Description = "Обновляет количество визитов пользователя.",
    OperationId = "Comment_Put")]
    public async Task<Result> UpdateUserVisitAsync(
        Guid userId,
        [FromServices] ICommandHandler<UpdateUserVisitCommand> handler ) =>
        await handler.Handle(new UpdateUserVisitCommand(userId), new CancellationToken(false));


    [HttpDelete("{userId}")]
    [SwaggerOperation(
    Summary = "Удалить пользователя.",
    Description = "Удаляет запись из таблиц UserIfo и UserStatistic(каскадно).",
    OperationId = "Comment_Delete")]
    public async Task<Result> DeleteUserInfoAsync(
        Guid userId,
        [FromServices] ICommandHandler<DeleteUserCommand> handler ) =>
        await handler.Handle(new DeleteUserCommand(userId), new CancellationToken(false));

}