using Abstractions.Commands;

using Ardalis.Result;

using Contracts.DTOs.QuestionService;

using QuestionService.Domain.Repositories;

namespace QuestionService.Application.Features.GetQuestionTitlesByIds;

public class GetQuestionTitlesByIdsHandler : ICommandHandler<IEnumerable<QuestionTitleDto>, GetQuestionTitlesByIdsCommand> {

  private readonly IQuestionServiceRepository _questionRepository;
  
  public GetQuestionTitlesByIdsHandler (IQuestionServiceRepository questionRepository) {
    _questionRepository = questionRepository;
  }

  public async Task<Result<IEnumerable<QuestionTitleDto>>> Handle (GetQuestionTitlesByIdsCommand command, CancellationToken cancellationToken) {
    var questions = await _questionRepository.GetQuestionTitlesByIdsAsync (command.QuestionIds, cancellationToken);
    
    if (!questions.IsSuccess)
      return Result<IEnumerable<QuestionTitleDto>>.Error (new ErrorList (questions.Errors));

    return Result<IEnumerable<QuestionTitleDto>>.Success (questions.Value);
  }

}