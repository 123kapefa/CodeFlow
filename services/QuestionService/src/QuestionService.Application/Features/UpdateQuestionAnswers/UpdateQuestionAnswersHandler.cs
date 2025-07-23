using Ardalis.Result;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Repositories;

using Abstractions.Commands;

namespace QuestionService.Application.Features.UpdateQuestionAnswers;

public class UpdateQuestionAnswersHandler : ICommandHandler<UpdateQuestionAnswersCommand> {

    private readonly IQuestionServiceRepository _questionServiceRepository;

    public UpdateQuestionAnswersHandler( IQuestionServiceRepository questionServiceRepository ) {
        _questionServiceRepository = questionServiceRepository;
    }

    public async Task<Result> Handle( UpdateQuestionAnswersCommand command, CancellationToken cancellationToken ) {

        Result<Question> questionResult = 
            await _questionServiceRepository.GetQuestionShortAsync(command.QuestionId, cancellationToken);

        if(!questionResult.IsSuccess)
            return Result.Error(new ErrorList(questionResult.Errors));

        questionResult.Value.AnswersCount += 1;

        Result updateResult = 
            await _questionServiceRepository.UpdateQuestionAsync(questionResult.Value, cancellationToken);

        
        return updateResult.IsSuccess ? Result.Success() : Result.Error(new ErrorList(updateResult.Errors));
    }
}
