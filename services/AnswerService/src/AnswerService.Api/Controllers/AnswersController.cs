using AnswerService.Application.Features.CreateAnswer;
using AnswerService.Application.Features.DeleteAnswer;
using AnswerService.Application.Features.EditAnswer;
using AnswerService.Application.Features.GetAnswersByQuestionId;
using AnswerService.Application.Features.GetAnswersByUserId;

using Ardalis.Result;
using Ardalis.Result.AspNetCore;

using Abstractions.Commands;

using AnswerService.Application.Features.UpdateAnswerAccept;

using Contracts.AnswerService.Requests;
using Contracts.AnswerService.Responses;

using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

namespace AnswerService.Api.Controllers;

[ApiController]
[Route ("[controller]")]
[TranslateResultToActionResult]
public class AnswersController : ControllerBase {

  [HttpPost]
  [SwaggerOperation(
    Summary = "Создать ответ.",
    Description = "Создает новый ответ в таблицу: Answers и AnswerChangingHistories.",
    OperationId = "Answer_Post")]
  [SwaggerResponse(200, "Успешное создание ответа", typeof(Result<CreateAnswerResponse>))]
  [SwaggerResponse(400, "Некорректный запрос")]

  public async Task<Result<CreateAnswerResponse>> CreateAnswer (
    [FromBody] CreateAnswerRequest request
    , [FromServices] ICommandHandler<CreateAnswerResponse, CreateAnswerCommand> handler) =>
    await handler.Handle (new CreateAnswerCommand (request), new CancellationToken (false));

  [HttpPost ("edit/{answerId:guid}")]
  [SwaggerOperation(
    Summary = "Редактировать ответ",
    Description = "Обновляет существующий ответ по его идентификатору",
    OperationId = "Answer_Edit")]
  [SwaggerResponse(200, "Ответ успешно отредактирован")]
  [SwaggerResponse(404, "Ответ не найден")]
  [SwaggerResponse(400, "Некорректный запрос")]
  public async Task<Result> EditAnswer (
    [FromRoute] Guid answerId
    , [FromBody] UpdateAnswerRequest request
    , [FromServices] ICommandHandler<EditAnswerCommand> handler) =>
    await handler.Handle (new EditAnswerCommand(answerId, request), new CancellationToken (false));

  [HttpDelete ("{answerId:guid}")]
  [SwaggerOperation(
    Summary = "Удалить ответ",
    Description = "Удаляет ответ по указанному идентификатору",
    OperationId = "Answer_Delete")]
  [SwaggerResponse(200, "Ответ успешно удален")]
  [SwaggerResponse(404, "Ответ не найден")]
  public async Task<Result> DeleteAnswer (
    [FromRoute] Guid answerId,
    [FromServices] ICommandHandler<DeleteAnswerCommand> handler) =>
  await handler.Handle (new DeleteAnswerCommand(answerId), new CancellationToken (false));

  [HttpGet ("question/{questionId:guid}")]
  [SwaggerOperation(
    Summary = "Получить ответы по ID вопроса",
    Description = "Возвращает все ответы для указанного вопроса",
    OperationId = "Answer_GetByQuestionId")]
  [SwaggerResponse(200, "Список ответов успешно получен", typeof(GetAnswersResponse))]
  [SwaggerResponse(404, "Вопрос не найден")]
  public async Task<GetAnswersResponse> GetAnswersByQuestionId (
    [FromRoute] Guid questionId
    , [FromServices] ICommandHandler<GetAnswersResponse, GetAnswersByQuestionIdCommand> handler) =>
    await handler.Handle (new GetAnswersByQuestionIdCommand(questionId), new CancellationToken (false));

  [HttpGet ("user/{userId:guid}")]
  [SwaggerOperation(
    Summary = "Получить ответы пользователя",
    Description = "Возвращает все ответы, созданные указанным пользователем",
    OperationId = "Answer_GetByUserId")]
  [SwaggerResponse(200, "Список ответов успешно получен", typeof(GetAnswersResponse))]
  [SwaggerResponse(404, "Пользователь не найден")]
  public async Task<GetAnswersResponse> GetAnswersByUserId (
    [FromRoute] Guid userId
    , [FromServices] ICommandHandler<GetAnswersResponse, GetAnswersByUserIdCommand> handler) =>
    await handler.Handle (new GetAnswersByUserIdCommand(userId), new CancellationToken (false));
  
  [HttpPut ("accept/{answerId:guid}/question/{questionId:guid}")]
  [SwaggerOperation(
    Summary = "Принять ответ",
    Description = "Отмечает ответ как принятый для указанного вопроса",
    OperationId = "Answer_Accept")]
  [SwaggerResponse(200, "Ответ успешно принят")]
  [SwaggerResponse(404, "Ответ или вопрос не найден")]
  [SwaggerResponse(400, "Некорректный запрос")]
  public async Task<Result> AcceptAnswer (
    [FromRoute] Guid answerId
    , [FromRoute] Guid questionId
    , [FromServices] ICommandHandler<UpdateAnswerAcceptCommand> handler) =>
  await handler.Handle (new UpdateAnswerAcceptCommand(answerId, questionId), new CancellationToken (false));
  
}