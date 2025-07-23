using Abstractions.Commands;

using Ardalis.Result;
using Ardalis.Result.AspNetCore;

using CommentService.Application.Features.CreateComment;
using CommentService.Application.Features.DeleteComment;
using CommentService.Application.Features.GetCommentById;
using CommentService.Application.Features.GetComments;
using CommentService.Application.Features.UpdateComment;
using CommentService.Domain.Enums;

using Contracts.CommentService.DTOs;

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


    [HttpPut("{commentId}")]
    [SwaggerOperation(
    Summary = "Обновить комментарий.",
    Description = "Обновляет запись в таблице Comments.",
    OperationId = "Comment_Put")]
    public async Task<Result> UpdateCommentAsync(
        Guid commentId,
        [FromBody]string content,
        [FromServices] ICommandHandler<UpdateCommentCommand> handler ) =>
        await handler.Handle(new UpdateCommentCommand(commentId, content), new CancellationToken(false));


    [HttpGet("{commentId}")]
    [SwaggerOperation(
    Summary = "Получить комментарий по commentId.",
    Description = "Возвращает объект CommentDTO .",
    OperationId = "Comment_Get")]
    public async Task<Result<CommentDTO>> GetCommentByIdAsync(
        Guid commentId,
        [FromServices] ICommandHandler<CommentDTO, GetCommentByIdCommand> handler ) =>
        await handler.Handle(new GetCommentByIdCommand(commentId), new CancellationToken(false));


    [HttpGet("question/{targetId}")]
    [SwaggerOperation(
    Summary = "Получить список комментариев для вопроса по targetId.",
    Description = "Возвращает IEnumeradle<CommentDTO> .",
    OperationId = "Comment_Get")]
    public async Task<Result<IEnumerable<CommentDTO>>> GetQuestionCommentsAsync(        
        Guid targetId,
        [FromServices] ICommandHandler<IEnumerable<CommentDTO>, GetCommentsCommand> handler ) =>
        await handler.Handle(new GetCommentsCommand(TypeTarget.Question, targetId), new CancellationToken(false));


    [HttpGet("answer/{targetId}")]
    [SwaggerOperation(
    Summary = "Получить список комментариев для ответа по targetId.",
    Description = "Возвращает IEnumeradle<CommentDTO> .",
    OperationId = "Comment_Get")]
    public async Task<Result<IEnumerable<CommentDTO>>> GetAnswerCommentsAsync(
       Guid targetId,
       [FromServices] ICommandHandler<IEnumerable<CommentDTO>, GetCommentsCommand> handler ) =>
       await handler.Handle(new GetCommentsCommand(TypeTarget.Answer, targetId), new CancellationToken(false));


    [HttpDelete("{commentId}")]
    [SwaggerOperation(
    Summary = "Удалить комментарий по commentId.",
    Description = "Удаляет запись из таблицы Comments .",
    OperationId = "Comment_Delete")]
    public async Task<Result> DeleteCommentByIdAsync(
        Guid commentId,
        [FromServices] ICommandHandler<DeleteCommentCommand> handler ) =>
        await handler.Handle(new DeleteCommentCommand(commentId), new CancellationToken(false));

}
