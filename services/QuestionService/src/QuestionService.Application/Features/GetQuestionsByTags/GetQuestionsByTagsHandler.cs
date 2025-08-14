using Abstractions.Commands;

using Ardalis.Result;

using Contracts.DTOs.QuestionService;

using QuestionService.Application.Extensions;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Repositories;

namespace QuestionService.Application.Features.GetQuestionsByTags;

public class GetQuestionsByTagsHandler : ICommandHandler<IEnumerable<QuestionShortDTO>, GetQuestionsByTagsCommand> {

  private readonly IQuestionServiceRepository _questionServiceRepository;

  public GetQuestionsByTagsHandler (IQuestionServiceRepository questionServiceRepository) {
    _questionServiceRepository = questionServiceRepository;
  }

  public async Task<Result<IEnumerable<QuestionShortDTO>>> Handle (
    GetQuestionsByTagsCommand command,
    CancellationToken cancellationToken) {
    var recommendedQuestions = await _questionServiceRepository.GetQuestionsByTagsAsync (
      command.TagIds, 
      command.PageParams,
      command.SortParams, 
      cancellationToken);
    
    if (!recommendedQuestions.IsSuccess)
      return Result<IEnumerable<QuestionShortDTO>>.Error (new ErrorList (recommendedQuestions.Errors));

    if (recommendedQuestions.Value.Count () < command.PageParams.PageSize) {
      var otherQuestions = await _questionServiceRepository.GetQuestionsWithoutIdsAsync (
        recommendedQuestions.Value.Select (q => q.Id),  
        (int)command.PageParams.PageSize - recommendedQuestions.Value.Count (),
        cancellationToken);

      List<Question> questions = recommendedQuestions.Value.ToList ();
      
      questions.AddRange (otherQuestions.Value);
      
      return Result.Success (questions.ToQuestionsShortDto());
    }
      
    return Result.Success (recommendedQuestions.Value.ToQuestionsShortDto());
  }

}