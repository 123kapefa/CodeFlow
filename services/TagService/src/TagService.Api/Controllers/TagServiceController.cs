using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Contracts.Commands;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TagService.Application.DTO;
using TagService.Application.Features.Tags.CreateTag;
using TagService.Application.Features.Tags.DeleteTag;
using TagService.Application.Features.Tags.GetTagById;
using TagService.Application.Features.Tags.GetTagByName;
using TagService.Application.Features.Tags.UpdateTag;
using TagService.Application.Features.Tags.UpdateTagRequest;
using TagService.Application.Features.WatchedTags.GetUserWatchedTags;


namespace TagService.Api.Controllers;

[ApiController]
[Route("tags")]
[TranslateResultToActionResult]
public class TagServiceController : ControllerBase{

    [HttpGet("{tagId}")]
    [SwaggerOperation(
    Summary = "Получить тэг по tag.Id.",
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
    Summary = "Получить список тэгов пользователя по userId.",
    Description = "Возвращает список объектов(IEnumerable<WatchedTagDTO>).",
    OperationId = "Tag_Get")]
    public async Task<Result<IEnumerable<WatchedTagDTO>>> GetUserWatchedTagsAsync(
        Guid userId,
        [FromServices] ICommandHandler<IEnumerable<WatchedTagDTO>, GetUserWatchedTagsCommand> handler ) =>
        await handler.Handle(new GetUserWatchedTagsCommand(userId), new CancellationToken(false));

}
