using Abstractions.Commands;

using Microsoft.AspNetCore.Mvc;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;

using Contracts.QuestionService.DTOs;
using Contracts.QuestionService.Request;

using QuestionService.Application.Features.GetQuestion;
using QuestionService.Application.Features.GetQuestionShort;
using QuestionService.Application.Features.GetQuestionHistory;
using QuestionService.Application.Features.CreateQuestion;
using QuestionService.Application.Features.GetQuestionTags;
using QuestionService.Application.Features.UpdateQuestion;
using QuestionService.Application.Features.DeleteQuestion;
using QuestionService.Application.Features.UpdateQuestionAccept;
using QuestionService.Application.Features.UpdateQuestionView;
using QuestionService.Application.Features.UpdateQuestionVote;
using QuestionService.Domain.Filters;
using QuestionService.Application.Features.GetQuestions;
using QuestionService.Application.Features.UpdateQuestionAnswers;
using QuestionService.Application.Features.GetUserQuestions;
using Swashbuckle.AspNetCore.Annotations;
using QuestionService.Application.Features.ReduceQuestionAnswers;

namespace QuestionService.Api.Contollers;

[ApiController]
[Route("questions")]
[TranslateResultToActionResult]
public class QuestionController : ControllerBase {


   
    [HttpGet("{questionId}")]
    [SwaggerOperation(
    Summary = "Получить вопрос по questionId.",
    Description = "Возвращает полный объект(QuestionDTO) с тегами и историей изменений.",
    OperationId = "Question_Get",
    Tags = new[] { "Question" })]
    public async Task<Result<QuestionDTO>> GetQuestionAsync(
        Guid questionId,
        [FromServices] ICommandHandler<QuestionDTO, GetQuestionCommand> handler ) =>
        await handler.Handle(new GetQuestionCommand(questionId), new CancellationToken(false));


    [HttpGet("short/{questionId}")] // TODO нужен для теста
    [SwaggerOperation(
    Summary = "Получить короткий вопрос по questionId.",
    Description = "Возвращает не полный объект(QuestionShortDTO) с тегами и историей изменений.",
    OperationId = "Question_Get")]
    public async Task<Result<QuestionShortDTO>> GetQuestionShortDTO(
        Guid questionId,
        [FromServices] ICommandHandler<QuestionShortDTO, GetQuestionShortCommand> handler ) =>
        await handler.Handle(new GetQuestionShortCommand(questionId), new CancellationToken(false));


    [HttpGet("{questionId}/history")]
    [SwaggerOperation(
    Summary = "Получить историю изменения вопроса по questionId.",
    Description = "Возвращает историю изменений вопроса(IEnumerable<QuestionHistoryDTO>) по questionId.",
    OperationId = "QuestionHistory_Get")]
    public async Task<Result<IEnumerable<QuestionHistoryDTO>>> GetQuestionHistoryAsync(
        Guid questionId,
        [FromServices] ICommandHandler<IEnumerable<QuestionHistoryDTO>, GetQuestionHistoryCommand> handler ) =>
        await handler.Handle(new GetQuestionHistoryCommand(questionId), new CancellationToken(false));


    [HttpGet("{questionId}/tag")]
    [SwaggerOperation(
    Summary = "Получить тэги вопроса questionId.",
    Description = "Возвращает список тэгов вопроса(IEnumerable<QuestionTagDTO>) по questionId.",
    OperationId = "QuestionTags_Get")]
    public async Task<Result<IEnumerable<QuestionTagDTO>>> GetQuestionTagsAsync(
        Guid questionId, 
        [FromServices]ICommandHandler<IEnumerable<QuestionTagDTO>, GetQuestionTagsCommand> handler) => 
        await handler.Handle(new GetQuestionTagsCommand(questionId), new CancellationToken(false));

    [HttpPost]
    [SwaggerOperation(
    Summary = "Создать вопрос.",
    Description = "Создает запись в таблицы: Questions, QuestionTags и QuestionChangingHistories.",
    OperationId = "Question_Post")]
    public async Task<Result> CreateQuestionAsync(
        [FromBody] CreateQuestionDTO createQuestionDTO,
        [FromServices] ICommandHandler<CreateQuestionCommand> handler ) =>
        await handler.Handle(new CreateQuestionCommand(createQuestionDTO), new CancellationToken(false));

    [HttpPut]
    [SwaggerOperation(
    Summary = "Обновить вопрос.",
    Description = "Обновляет записи в таблицах: Questions, QuestionTags и QuestionChangingHistories.",
    OperationId = "Question_Put")]
    public async Task<Result> UpdateQuestionAsync(
        [FromBody] UpdateQuestionDTO updateQuestionDTO,
        [FromServices] ICommandHandler<UpdateQuestionCommand> handler ) =>
        await handler.Handle(new UpdateQuestionCommand(updateQuestionDTO), new CancellationToken(false));

    [HttpDelete("{questionId}")]
    [SwaggerOperation(
    Summary = "Удалить вопрос (каскадно).",
    Description = "Удаляет записи в таблицах: Questions, QuestionTags и QuestionChangingHistories.",
    OperationId = "Question_Delete")]
    public async Task<Result> DeleteQuestionAsync(
        Guid questionId,
        [FromServices] ICommandHandler<DeleteQuestionCommand> handler ) =>
        await handler.Handle(new DeleteQuestionCommand(questionId), new CancellationToken(false));

    [HttpPut("{questionId}/answer-accept")]
    [SwaggerOperation(
    Summary = "Обновить поле IsClosed и AcceptedAnswerId.",
    Description = "Обновляет данные в таблице Questions.",
    OperationId = "Question_Put")]
    public async Task<Result> UpdateQuestionAcceptAsync(
        [FromRoute] Guid questionId,
        [FromBody] UpdateQuestionAcceptRequest request,
        [FromServices] ICommandHandler<UpdateQuestionAcceptCommand> handler ) =>
        await handler.Handle(new UpdateQuestionAcceptCommand(questionId, request.AcceptAnswerId, request.UserAnswerId), new CancellationToken(false));


    [HttpPut("{questionId}/views")]
    [SwaggerOperation(
    Summary = "Обновить поле ViewsCount.",
    Description = "Обновляет данные в таблице Questions.",
    OperationId = "Question_Put")]
    public async Task<Result> UpdateQuestionViewAsync(
        Guid questionId,        
        [FromServices] ICommandHandler<UpdateQuestionViewCommand> handler ) =>
        await handler.Handle(new UpdateQuestionViewCommand(questionId), new CancellationToken(false));


    [HttpPut("{questionId}/vote/{value}")]
    [SwaggerOperation(
    Summary = "Обновить поле Upvotes или Downvotes.",
    Description = "Обновляет данные в таблице Questions.",
    OperationId = "Question_Put")]
    public async Task<Result> UpdateQuestionVoteAsync(
        Guid questionId,
        int value,
        [FromServices] ICommandHandler<UpdateQuestionVoteCommand> handler ) =>
        await handler.Handle(new UpdateQuestionVoteCommand(questionId, value), new CancellationToken(false));

    [HttpPut("{questionId}/answers")]
    [SwaggerOperation(
    Summary = "Обновить поле AnswersCount.",
    Description = "Обновляет данные в таблице Questions.",
    OperationId = "Question_Put")]
    public async Task<Result> UpdateQuestionAnswersAsync(
        Guid questionId,
        [FromServices] ICommandHandler<UpdateQuestionAnswersCommand> handler ) =>
        await handler.Handle(new UpdateQuestionAnswersCommand(questionId), new CancellationToken(false));

    [HttpGet]
    [SwaggerOperation(
    Summary = "Получить по заданным параметрам список вопросов .",
    Description = "Возвращает PagedResult<IEnumerable<QuestionShortDTO>>> .",
    OperationId = "Questions_Get")]
    public async Task<Result<PagedResult<IEnumerable<QuestionShortDTO>>>> GetQuestionsAsync(
      [FromQuery] PageParams pageParams,
      [FromQuery] SortParams sortParams,
      [FromServices] ICommandHandler<PagedResult<IEnumerable<QuestionShortDTO>>, GetQuestionsCommand> handler ) =>
      await handler.Handle(new GetQuestionsCommand(pageParams, sortParams), new CancellationToken(false));

    [HttpGet("user/{userId}")]
    [SwaggerOperation(
    Summary = "Получить по заданным параметрам список вопросов пользователя по userId.",
    Description = "Возвращает PagedResult<IEnumerable<QuestionShortDTO>>> .",
    OperationId = "Questions_Get")]
    public async Task<Result<PagedResult<IEnumerable<QuestionShortDTO>>>> GetUserQuestionsAsync(
      Guid userId,
      [FromQuery] PageParams pageParams,
      [FromQuery] SortParams sortParams,
      [FromServices] ICommandHandler<PagedResult<IEnumerable<QuestionShortDTO>>, GetUserQuestionsCommand> handler ) =>
      await handler.Handle(new GetUserQuestionsCommand(userId, pageParams, sortParams), new CancellationToken(false));

    [HttpPut("{questionId}/answer/reduce")]
    [SwaggerOperation(
    Summary = "Изменить AnswersCount в Questions при удлении ответа.",
    Description = "Возвращает PagedResult<IEnumerable<QuestionShortDTO>>> .",
    OperationId = "Questions_Put")]
    public async Task<Result> ReduceQuestionAnswersAsync(
        Guid questionId, 
        [FromServices]ICommandHandler<ReduceQuestionAnswersCommand> handler) =>
        await handler.Handle(new ReduceQuestionAnswersCommand(questionId), new CancellationToken(false));

}

// user 1 01985fd7-f48d-79c5-ba32-5b2c14cb7d02  testrabbitmq@gmail.com
// user 2 01985fdb-9b87-7e8d-91da-bcebf52c9687  user2@gmail.com