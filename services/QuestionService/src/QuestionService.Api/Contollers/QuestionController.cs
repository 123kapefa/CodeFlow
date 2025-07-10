using Microsoft.AspNetCore.Mvc;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using QuestionService.Domain.Entities;
using QuestionService.Application.Features.GetQuestion;
using Contracts.Commands;
using QuestionService.Application.DTO;
using QuestionService.Application.Features.GetQuestionShort;
using QuestionService.Application.Features.GetQuestionHistory;

namespace QuestionService.Api.Contollers;

[ApiController]
[Route("question")]
public class QuestionController : ControllerBase {


    [HttpGet("{questionId}")]
    public async Task<Result<QuestionDTO>> GetQuestionAsync(
        Guid questionId,
        [FromServices] ICommandHandler<QuestionDTO, GetQuestionCommand> handler ) =>
        await handler.Handle(new GetQuestionCommand(questionId), new CancellationToken(false));

    [HttpGet("short/{questionId}")]
    public async Task<Result<QuestionShortDTO>> GetQuestionShortDTO(
        Guid questionId,
        [FromServices] ICommandHandler<QuestionShortDTO, GetQuestionShortCommand> handler ) =>
        await handler.Handle(new GetQuestionShortCommand(questionId), new CancellationToken(false));

    [HttpGet("{questionId}/history")]
    public async Task<Result<IEnumerable<QuestionHistoryDTO>>> GetQuestionHistoryAsync(
        Guid questionId,
        [FromServices] ICommandHandler<IEnumerable<QuestionHistoryDTO>, GetQuestionHistoryCommand> handler ) =>
        await handler.Handle(new GetQuestionHistoryCommand(questionId), new CancellationToken(false));

}
