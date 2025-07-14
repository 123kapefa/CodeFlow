using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using CommentService.Application.DTO;
using CommentService.Application.Features.CreateComment;
using CommentService.Application.Features.DeleteCommentById;
using CommentService.Application.Features.GetCommentById;
using Contracts.Commands;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CommentService.Api.Controllers;

[ApiController]
[Route("comments")]
[TranslateResultToActionResult]
public class CommentController : ControllerBase {


    [HttpPost]
    [SwaggerOperation(
    Summary = "Создать комментарий.",
    Description = "Создает запись в таблице Comments.",
    OperationId = "Comment_Post")]
    public async Task<Result> CreateCommentAsync(
        [FromBody] CreateCommentDTO createCommentDTO,
        [FromServices] ICommandHandler<CreateCommentCommand> handler ) =>
        await handler.Handle(new CreateCommentCommand(createCommentDTO), new CancellationToken(false));


    [HttpGet("{commentId}")]
    [SwaggerOperation(
    Summary = "Получить комментарий по commentId.",
    Description = "Возвращает объект CommentDTO .",
    OperationId = "Comment_Get")]
    public async Task<Result<CommentDTO>> GetCommentByIdAsync(
        Guid commentId,
        [FromServices] ICommandHandler<CommentDTO, GetCommentByIdCommand> handler ) =>
        await handler.Handle(new GetCommentByIdCommand(commentId), new CancellationToken(false));


    [HttpDelete("{commentId}")]
    [SwaggerOperation(
    Summary = "Удалить комментарий по commentId.",
    Description = "Удаляет запись из таблицы Comments .",
    OperationId = "Comment_Delete")]
    public async Task<Result> DeleteCommentByIdAsync(
        Guid commentId,
        [FromServices] ICommandHandler<DeleteCommentByIdCommand> handler ) =>
        await handler.Handle(new DeleteCommentByIdCommand(commentId), new CancellationToken(false));


}
