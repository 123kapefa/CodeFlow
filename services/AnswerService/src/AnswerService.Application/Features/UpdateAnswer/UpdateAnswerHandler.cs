using AnswerService.Domain.Entities;
using AnswerService.Domain.Repositories;

using Ardalis.Result;
using Ardalis.Result.FluentValidation;

using Contracts.Commands;

using FluentValidation;

namespace AnswerService.Application.Features.UpdateAnswer;

public class UpdateAnswerHandler : ICommandHandler<UpdateAnswerCommand> {

  private readonly IAnswerRepository _answerRepository;
  private readonly IAnswerChangingHistoryRepository _answerHistoryRepository;
  private readonly IValidator<UpdateAnswerCommand> _validator;
  
  public UpdateAnswerHandler (
    IAnswerRepository answerRepository, 
    IAnswerChangingHistoryRepository answerHistoryRepository, 
    IValidator<UpdateAnswerCommand> validator) {
    _answerRepository = answerRepository;
    _answerHistoryRepository = answerHistoryRepository;
    _validator = validator;
  }

  public async Task<Result> Handle (UpdateAnswerCommand command, CancellationToken ct) {
    
    var validationResult = await _validator.ValidateAsync(command, ct);

    if (!validationResult.IsValid) {
      return Result.Invalid(validationResult.AsErrors());
    }
    
    var answer = await _answerRepository.GetByIdAsync (command.Id, ct);
    
    if (!answer.IsSuccess)
      return Result.Error (new ErrorList(answer.Errors));

    var newAnswerChangingHistory = AnswerChangingHistory
     .Create (command.Id, command.Request.EditedUserId, command.Request.Content);

    var resultAdd = await _answerHistoryRepository.CreateAsync (newAnswerChangingHistory, ct);
    
    if (!resultAdd.IsSuccess)
      return Result.Error (new ErrorList(resultAdd.Errors));

    var resultEdit = await _answerRepository.EditAsync (newAnswerChangingHistory, ct);
    
    return resultEdit.IsSuccess ? Result.Success () : Result.Error (new ErrorList (resultEdit.Errors));
  }

}