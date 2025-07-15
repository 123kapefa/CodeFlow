using AnswerService.Application.Responses;
using AnswerService.Domain.Entities;
using AnswerService.Domain.Repositories;

using Ardalis.Result;
using Ardalis.Result.FluentValidation;

using Contracts.Commands;

using FluentValidation;

namespace AnswerService.Application.Features.CreateAnswer;

public class CreateAnswerHandler : ICommandHandler<CreateAnswerResponse, CreateAnswerCommand> {

  private readonly IAnswerRepository _answerRepository;
  private readonly IValidator<CreateAnswerCommand> _validator;
  
  public CreateAnswerHandler (IAnswerRepository answerRepository, IValidator<CreateAnswerCommand> validator) {
    _answerRepository = answerRepository;
    _validator = validator;
  }

  public async Task<Result<CreateAnswerResponse>> Handle (CreateAnswerCommand command, CancellationToken cancellationToken) {

    var validationResult = await _validator.ValidateAsync(command, cancellationToken);

    if (!validationResult.IsValid) {
      return Result<CreateAnswerResponse>.Invalid (validationResult.AsErrors ());
    }
    
    var newAnswer = Answer.Create(command.Request.QuestionId, command.Request.UserId, command.Request.Content);
    
    var result = await _answerRepository.AddAsync (newAnswer, cancellationToken);
    
    if (!result.IsSuccess)
      return Result<CreateAnswerResponse>.Error (new ErrorList(result.Errors));
    
    return Result<CreateAnswerResponse>.Success (new CreateAnswerResponse());

  }

}