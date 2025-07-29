using AnswerService.Domain.Entities;
using AnswerService.Domain.Repositories;

using Ardalis.Result;
using Ardalis.Result.FluentValidation;

using Abstractions.Commands;

using FluentValidation;

namespace AnswerService.Application.Features.EditAnswer;

public class EditAnswerHandler : ICommandHandler<EditAnswerCommand> {

  private readonly IAnswerRepository _answerRepository;
  private readonly IAnswerChangingHistoryRepository _answerHistoryRepository;
  private readonly IValidator<EditAnswerCommand> _validator;
  
  public EditAnswerHandler (
    IAnswerRepository answerRepository, 
    IAnswerChangingHistoryRepository answerHistoryRepository, 
    IValidator<EditAnswerCommand> validator) {
    _answerRepository = answerRepository;
    _answerHistoryRepository = answerHistoryRepository;
    _validator = validator;
  }

  public async Task<Result> Handle (EditAnswerCommand command, CancellationToken ct) { 
    
    var validationResult = await _validator.ValidateAsync(command, ct);

    if (!validationResult.IsValid)
      return Result.Invalid(validationResult.AsErrors());
    
    var answer = await _answerRepository.GetByIdAsync (command.Id, ct);
    
    if (!answer.IsSuccess)
      return Result.Error (new ErrorList(answer.Errors));

    var newAnswerChangingHistory = AnswerChangingHistory
     .Create (command.Id, command.Request.EditedUserId, command.Request.Content);
    
    var resultUpdate = await _answerRepository.UpdateAsync(answer.Value, newAnswerChangingHistory, ct);
    
    return resultUpdate.IsSuccess ? Result.Success () : Result.Error (new ErrorList (resultUpdate.Errors));
  }

}