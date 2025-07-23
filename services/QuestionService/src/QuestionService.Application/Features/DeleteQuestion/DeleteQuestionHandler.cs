using Ardalis.Result;
using QuestionService.Domain.Repositories;

using Abstractions.Commands;

namespace QuestionService.Application.Features.DeleteQuestion;

public class DeleteQuestionHandler : ICommandHandler<DeleteQuestionCommand> {

    private readonly IQuestionServiceRepository _questionServiceRepository;

    public DeleteQuestionHandler( IQuestionServiceRepository questionServiceRepository ) {
        _questionServiceRepository = questionServiceRepository;
    }

    public async Task<Result> Handle( DeleteQuestionCommand command, CancellationToken cancellationToken ) {

        Result result = await _questionServiceRepository.DeleteQuestionAsync(command.QuestionId, cancellationToken);

        return result.IsSuccess ? Result.Success() : Result.Error(new ErrorList(result.Errors));
    }
}
