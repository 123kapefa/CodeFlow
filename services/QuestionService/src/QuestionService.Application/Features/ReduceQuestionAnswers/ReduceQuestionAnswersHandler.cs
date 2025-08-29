using Abstractions.Commands;
using Ardalis.Result;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Repositories;

namespace QuestionService.Application.Features.ReduceQuestionAnswers;

public class ReduceQuestionAnswersHandler : ICommandHandler<ReduceQuestionAnswersCommand> {

    private readonly IQuestionServiceRepository _repository;

    public ReduceQuestionAnswersHandler( IQuestionServiceRepository repository ) {
        _repository = repository;
    }

    public async Task<Result> Handle( ReduceQuestionAnswersCommand command, CancellationToken cancellationToken ) {

        if(command.QuestionId == Guid.Empty)
            return Result.Error("Question ID cannot be empty.");

        Result<Question> questionResult = await _repository.GetQuestionShortAsync(command.QuestionId, cancellationToken);
        if(!questionResult.IsSuccess)
            return Result.Error(new ErrorList(questionResult.Errors));

        if(questionResult.Value.AnswersCount <= 0)
            return Result.Error("Cannot reduce answers count below zero.");

        questionResult.Value.AnswersCount -= 1;

        Result updateResult = await _repository.UpdateQuestionAsync(questionResult.Value, cancellationToken);

        return updateResult.IsSuccess ? Result.Success() : Result.Error(new ErrorList(updateResult.Errors));
    }
}
