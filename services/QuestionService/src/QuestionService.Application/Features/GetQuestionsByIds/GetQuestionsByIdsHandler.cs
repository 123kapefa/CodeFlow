using Abstractions.Commands;

using Ardalis.Result;

using Contracts.DTOs.QuestionService;

using QuestionService.Application.Extensions;
using QuestionService.Application.Features.GetQuestions;
using QuestionService.Domain.Repositories;

namespace QuestionService.Application.Features.GetQuestionsByIds;

public class GetQuestionsByIdsHandler : ICommandHandler<PagedResult<IEnumerable<QuestionShortDTO>>, GetQuestionsByIdsCommand> {

  private readonly IQuestionServiceRepository _questionServiceRepository;

  public GetQuestionsByIdsHandler (IQuestionServiceRepository questionServiceRepository) {
    _questionServiceRepository = questionServiceRepository;
  }

  public async Task<Result<PagedResult<IEnumerable<QuestionShortDTO>>>> Handle (
    GetQuestionsByIdsCommand command,
    CancellationToken cancellationToken) {
    var result = await _questionServiceRepository.GetQuestionsByIdsAsync (command.QuestionIds, command.PageParams,
      command.SortParams, cancellationToken);
    
    if (!result.IsSuccess)
      return Result<PagedResult<IEnumerable<QuestionShortDTO>>>.Error (new ErrorList (result.Errors));

    return Result<PagedResult<IEnumerable<QuestionShortDTO>>>.Success (
      new PagedResult<IEnumerable<QuestionShortDTO>> (result.Value.pageInfo, result.Value.items.ToQuestionsShortDto()));
  }

}