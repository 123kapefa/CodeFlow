using Ardalis.Result;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Repositories;

using Abstractions.Commands;

using Contracts.DTOs.QuestionService;

using Microsoft.AspNetCore.Http;

using QuestionService.Application.Extensions;

namespace QuestionService.Application.Features.GetQuestion;

public class GetQuestionHandler : ICommandHandler<QuestionDTO, GetQuestionCommand> {

    private readonly IQuestionServiceRepository _questionServiceRepository;

    public GetQuestionHandler( IQuestionServiceRepository questionServiceRepository ) {
        _questionServiceRepository = questionServiceRepository;
    }

    public async Task<Result<QuestionDTO>> Handle( GetQuestionCommand command, CancellationToken cancellationToken ) {

        Result<Question> result = await _questionServiceRepository.GetQuestionAsync(command.QuestionId, cancellationToken);

        if(!result.IsSuccess)
            Result<QuestionDTO>.Error(new ErrorList(result.Errors));

        return Result<QuestionDTO>.Success(result.Value.ToQuestionDto ()) ;
    }
}
