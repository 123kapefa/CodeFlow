using Abstractions.Commands;

using Ardalis.Result;
using Ardalis.Result.AspNetCore;

using Contracts.TagService;

using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TagService.Application.Features.ParticipationTags.CreateTags;
using TagService.Application.Features.ParticipationTags.DeleteUserTags;
using TagService.Application.Features.ParticipationTags.GetUserTags;
using TagService.Application.Features.ParticipationTags.UpdateTags;
using TagService.Application.Features.Tags.CreateTag;
using TagService.Application.Features.Tags.DeleteTag;
using TagService.Application.Features.Tags.GetTagById;
using TagService.Application.Features.Tags.GetTagByName;
using TagService.Application.Features.Tags.GetTags;
using TagService.Application.Features.Tags.UpdateTag;
using TagService.Application.Features.Tags.UpdateTagCountQuestion;
using TagService.Application.Features.Tags.UpdateTagRequest;
using TagService.Application.Features.Tags.UpdateTagWatchers;
using TagService.Application.Features.WatchedTags.CreateWatchedTag;
using TagService.Application.Features.WatchedTags.DeleteUserWatchedTags;
using TagService.Application.Features.WatchedTags.DeleteWatchedTag;
using TagService.Application.Features.WatchedTags.GetUserWatchedTags;
using TagService.Domain.Filters;


namespace TagService.Api.Controllers;

[ApiController]
[Route("tags")]
[TranslateResultToActionResult]
public class TagServiceController : ControllerBase{

    [HttpGet("{tagId}")]
    [SwaggerOperation(
    Summary = "Получить тэг по tag.ID.",
    Description = "Возвращает объект(TagDTO).",
    OperationId = "Tag_Get")]
    public async Task<Result<TagDTO>> GetTagByIdAsync(
        int tagId,
        [FromServices] ICommandHandler<TagDTO,GetTagByIdCommand> handler ) =>
        await handler.Handle(new GetTagByIdCommand(tagId), new CancellationToken(false));

    [HttpGet("{tagName}/name")]
    [SwaggerOperation(
    Summary = "Получить тэг по tag.Name.",
    Description = "Возвращает объект(TagDTO).",
    OperationId = "Tag_Get")]
    public async Task<Result<TagDTO>> GetTagByNameAsync(
        string tagName,
        [FromServices] ICommandHandler<TagDTO, GetTagByNameCommand> handler ) =>
        await handler.Handle(new GetTagByNameCommand(tagName), new CancellationToken(false));

    [HttpGet]
    [SwaggerOperation(
    Summary = "Получить список тэгов.",
    Description = "Получает список тэгов для указанной страницы.",
    OperationId = "Tag_Get")]
    public async Task<Result<PagedResult<IEnumerable<TagDTO>>>> GetTagsAsync(
        [FromQuery] PageParams pageParams,
        [FromQuery] SortParams sortParams,
        [FromServices] ICommandHandler<PagedResult<IEnumerable<TagDTO>>, GetTagsCommand> handler ) =>
        await handler.Handle(new GetTagsCommand(pageParams, sortParams), new CancellationToken(false));


    [HttpPost]
    [SwaggerOperation(
    Summary = "Создать тэг.",
    Description = "Создает запись в таблице tags.",
    OperationId = "Tag_Post")]
    public async Task<Result> CreateTagAsync(
        [FromBody] TagCreateDTO tagCreateDTO,
        [FromServices] ICommandHandler<CreateTagCommand> handler ) =>
        await handler.Handle(new CreateTagCommand(tagCreateDTO), new CancellationToken(false));


    [HttpPut("{tagId}")]
    [SwaggerOperation(
    Summary = "Обновить тэг(имя и описание).",
    Description = "Обновляет запись в таблице tags.",
    OperationId = "Tag_Put")]
    public async Task<Result> UpdateTagAsync(
        int tagId,
        [FromBody] TagUpdateDTO tagUpdateDTO,
        [FromServices] ICommandHandler<UpdateTagCommand> handler ) =>
        await handler.Handle(new UpdateTagCommand(tagId, tagUpdateDTO), new CancellationToken(false));


    [HttpPut("request/{tagName}/daily")]
    [SwaggerOperation(
    Summary = "Обновить тэг(количество запросов за день).",
    Description = "Обновляет запись в таблице tags.",
    OperationId = "Tag_Put")]
    public async Task<Result> UpdateTagDailyRequestAsync(
        string tagName,
        [FromServices] ICommandHandler<UpdateTagRequestCommand> handler ) =>
        await handler.Handle(new UpdateTagRequestCommand(tagName), new CancellationToken(false));


    [HttpPut("request/{tagName}/{count}")]
    [SwaggerOperation(
    Summary = "Обновить тэг(количество вопросов).",
    Description = "Обновляет запись в таблице tags.",
    OperationId = "Tag_Put")]
    public async Task<Result> UpdateTagCountQuestionAsync(
        string tagName, 
        int count, 
        [FromServices]ICommandHandler<UpdateTagCountQuestionCommand> handler) =>
        await handler.Handle(new UpdateTagCountQuestionCommand(tagName, count), new CancellationToken(false));


    [HttpPut("request/{tagId}/watcher/{count}")]
    [SwaggerOperation(
     Summary = "Обновить тэг(количество наблюдателей).",
     Description = "Обновляет запись в таблице tags.",
     OperationId = "Tag_Put")]
    public async Task<Result> UpdateTagWatchersAsync(
     int tagId,
     int count,
     [FromServices] ICommandHandler<UpdateTagWatchersCommand> handler ) =>
     await handler.Handle(new UpdateTagWatchersCommand(tagId, count), new CancellationToken(false));


    [HttpDelete("{tagId}")]
    [SwaggerOperation(
    Summary = "Удалить тэг.",
    Description = "Удаляет запись в таблице tags.",
    OperationId = "Tag_Put")]
    public async Task<Result> DeleteTagAsync(
        int tagId,
        [FromServices] ICommandHandler<DeleteTagCommand> handler ) =>
        await handler.Handle(new DeleteTagCommand(tagId), new CancellationToken(false));




    [HttpGet("watched/{userId}")]
    [SwaggerOperation(
    Summary = "Получить список отслеживаемых тэгов пользователя по userID.",
    Description = "Возвращает список объектов(IEnumerable<WatchedTagDTO>).",
    OperationId = "Tag_Get")]
    public async Task<Result<IEnumerable<WatchedTagDTO>>> GetUserWatchedTagsAsync(
        Guid userId,
        [FromServices] ICommandHandler<IEnumerable<WatchedTagDTO>, GetUserWatchedTagsCommand> handler ) =>
        await handler.Handle(new GetUserWatchedTagsCommand(userId), new CancellationToken(false));


    [HttpPost("watched/{userId}/{tagId}")]
    [SwaggerOperation(
    Summary = "Создать отслеживаемый тэг.",
    Description = "Создает запись в таблице WatchedTags.",
    OperationId = "Tag_Post")]
    public async Task<Result> CreateWatchedTagAsync(
        Guid userId,
        int tagId,
        [FromServices] ICommandHandler<CreateWatchedTagCommand> handler ) =>
        await handler.Handle(new CreateWatchedTagCommand(userId, tagId), new CancellationToken(false));


    [HttpDelete("watched/{tagId}/{userId}")]
    [SwaggerOperation(
    Summary = "Удалить отслеживаемый тэг.",
    Description = "Удаляет запись в таблице WatchedTags.",
    OperationId = "Tag_Delete")]
    public async Task<Result> DeleteWatchedTagAsync(
        int tagId,
        Guid userId,
        [FromServices] ICommandHandler<DeleteWatchedTagCommand> handler ) =>
        await handler.Handle(new DeleteWatchedTagCommand(tagId, userId), new CancellationToken(false));


    [HttpDelete("watched/{userId}")]
    [SwaggerOperation(
    Summary = "Удалить все отслеживаемые тэги пользователя по userID.",
    Description = "Удаляет записи в таблице WatchedTags для определенного пользователя.",
    OperationId = "Tag_Delete")]
    public async Task<Result> DeleteUserWatchedTagsAsync(
        Guid userId,
        [FromServices] ICommandHandler<DeleteUserWatchedTagsCommand> handler ) =>
        await handler.Handle(new DeleteUserWatchedTagsCommand(userId), new CancellationToken(false));




    [HttpGet("participation")]
    [SwaggerOperation(
    Summary = "Получить обсуждаемые тэги для пользователя по userID.",
    Description = "Получает список тэгов для указанной страницы.",
    OperationId = "Tag_Get")]
    public async Task<Result<PagedResult<IEnumerable<ParticipationDTO>>>> GetUserParticipationTAgsAsync(
        [FromQuery]Guid userId,
        [FromQuery] PageParams pageParams,
        [FromQuery] SortParams sortParams,
        [FromServices] ICommandHandler<PagedResult<IEnumerable<ParticipationDTO>>, GetUserTagsCommand> handler ) =>
        await handler.Handle(new GetUserTagsCommand(userId, pageParams, sortParams), new CancellationToken(false));


    [HttpPost("participation")]
    [SwaggerOperation(
    Summary = "Создать обсуждаемый тэг.",
    Description = "Создает запись в таблицах UserTagParticipation и UserTagParticipationQuestion.",
    OperationId = "Tag_Post")]
    public async Task<Result> CreateUserTagParticipationAsync(
        [FromBody] CreateParticipationDto createParticipationDto,
        [FromServices] ICommandHandler<CreateTagsCommand> handler ) =>
        await handler.Handle(new CreateTagsCommand(createParticipationDto), new CancellationToken(false));


    [HttpPut("participation")]
    [SwaggerOperation(
    Summary = "Обновить обсуждаемый тэг.",
    Description = "Обновляет запись в таблицах UserTagParticipation и UserTagParticipationQuestion.",
    OperationId = "Tag_Put")]
    public async Task<Result> UpdateTagParticipationAsync(
        [FromBody] UpdateParticipationDto participationDto,
        [FromServices] ICommandHandler<UpdateTagsCommand> handler ) =>
        await handler.Handle(new UpdateTagsCommand(participationDto), new CancellationToken(false));


    [HttpDelete("participation/{userId}")]
    [SwaggerOperation(
    Summary = "Удаляет обсуждаемые тэги пользователя по userID.",
    Description = "Удаляет записи в таблицах UserTagParticipation и UserTagParticipationQuestion.",
    OperationId = "Tag_Delete")]
    public async Task<Result> DeleteUserTagsParticipationAsync(
        Guid userId,
        [FromServices] ICommandHandler<DeleteUserTagsCommand> handler ) =>
        await handler.Handle(new DeleteUserTagsCommand(userId), new CancellationToken(false));

}
