using AnswerService.Domain.Entities;
using AnswerService.Domain.Repositories;

using Ardalis.Result;
using Ardalis.Result.FluentValidation;

using Abstractions.Commands;

using Contracts.AnswerService.Responses;
using Contracts.Publishers.AnswerService;

using FluentValidation;

using Messaging.Broker;

namespace AnswerService.Application.Features.CreateAnswer;

public class CreateAnswerHandler : ICommandHandler<CreateAnswerResponse, CreateAnswerCommand> {

  private readonly IMessageBroker _messageBroker;
  private readonly IAnswerRepository _answerRepository;
  private readonly IValidator<CreateAnswerCommand> _validator;

  public CreateAnswerHandler (
    IAnswerRepository answerRepository
    , IValidator<CreateAnswerCommand> validator
    , IMessageBroker messageBroker) {
    _answerRepository = answerRepository;
    _validator = validator;
    _messageBroker = messageBroker;
  }

  public async Task<Result<CreateAnswerResponse>> Handle (
    CreateAnswerCommand command
    , CancellationToken cancellationToken) {
    var validationResult = await _validator.ValidateAsync (command, cancellationToken);

    if (!validationResult.IsValid) {
      return Result<CreateAnswerResponse>.Invalid (validationResult.AsErrors ());
    }

    var newAnswer = Answer.Create (command.Request.QuestionId, command.Request.UserId, command.Request.Content);

    var newAnswerChangingHistory =
      AnswerChangingHistory.Create (newAnswer.Id, command.Request.UserId, command.Request.Content);

    var result = await _answerRepository.CreateAsync (newAnswer, newAnswerChangingHistory, cancellationToken);

    if (!result.IsSuccess)
      return Result<CreateAnswerResponse>.Error (new ErrorList (result.Errors));

    await _messageBroker.PublishAsync (new AnswerCreated (newAnswer.QuestionId), cancellationToken);
    await _answerRepository.SaveAsync (cancellationToken);
    
    return Result<CreateAnswerResponse>.Success (new CreateAnswerResponse ());
  }

}