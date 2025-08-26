using Abstractions.Commands;

using Ardalis.Result;

using Contracts.DTOs.QuestionService;

using QuestionService.Application.Extensions;
using QuestionService.Application.Features.GetQuestions;
using QuestionService.Domain.Repositories;

namespace QuestionService.Application.Features.GetQuestionsByIds;

public class GetQuestionsByIdsHandler : ICommandHandler<IEnumerable<QuestionShortDTO>, GetQuestionsByIdsCommand> {

  private readonly IQuestionServiceRepository _questionServiceRepository;

  public GetQuestionsByIdsHandler (IQuestionServiceRepository questionServiceRepository) {
    _questionServiceRepository = questionServiceRepository;
  }

  public async Task<Result<IEnumerable<QuestionShortDTO>>> Handle (
    GetQuestionsByIdsCommand command,
    CancellationToken cancellationToken) {
    var result = await _questionServiceRepository.GetQuestionsByIdsAsync (command.QuestionIds, cancellationToken);
    
    if (!result.IsSuccess)
      return Result<IEnumerable<QuestionShortDTO>>.Error (new ErrorList (result.Errors));

    return Result<IEnumerable<QuestionShortDTO>>.Success (
      new List<QuestionShortDTO> (result.Value.ToQuestionsShortDto()));
  }

} 