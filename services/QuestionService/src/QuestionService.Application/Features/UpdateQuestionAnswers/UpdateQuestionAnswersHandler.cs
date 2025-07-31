using Ardalis.Result;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Repositories;

using Abstractions.Commands;

using Microsoft.Extensions.Logging;

namespace QuestionService.Application.Features.UpdateQuestionAnswers;

public class UpdateQuestionAnswersHandler : ICommandHandler<UpdateQuestionAnswersCommand> {

    private readonly IQuestionServiceRepository _questionServiceRepository;
    private readonly ILogger<UpdateQuestionAnswersHandler> _logger;

    public UpdateQuestionAnswersHandler( IQuestionServiceRepository questionServiceRepository, ILogger<UpdateQuestionAnswersHandler> logger) {
        _questionServiceRepository = questionServiceRepository;
        _logger = logger;
    }

    public async Task<Result> Handle( UpdateQuestionAnswersCommand command, CancellationToken cancellationToken ) {

        Result<Question> questionResult = 
            await _questionServiceRepository.GetQuestionShortAsync(command.QuestionId, cancellationToken);

        if(!questionResult.IsSuccess)
            return Result.Error(new ErrorList(questionResult.Errors));

        questionResult.Value.AnswersCount += 1;
        await _questionServiceRepository.SaveChangesAsync (cancellationToken);
        _logger.LogInformation("Answers count updated");

        Result updateResult = 
            await _questionServiceRepository.UpdateQuestionAsync(questionResult.Value, cancellationToken);

        
        return updateResult.IsSuccess ? Result.Success() : Result.Error(new ErrorList(updateResult.Errors));
    }
}
