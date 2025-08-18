using Abstractions.Commands;

using Ardalis.Result;

using Contracts.Publishers.ReputationService;

using Messaging.Broker;

using ReputationService.Domain.Entities;
using ReputationService.Domain.Policies;
using ReputationService.Domain.Repositories;

namespace ReputationService.Application.Features.AcceptedAnswerChanged;

public class AcceptedAnswerChangedHandler : ICommandHandler<AcceptedAnswerChangedCommand> {

  private readonly IReputationRepository _repository;
  private readonly IReputationPolicy _policy;
  private readonly IMessageBroker _messageBroker;

  public AcceptedAnswerChangedHandler (
    IReputationRepository repository,
    IReputationPolicy policy,
    IMessageBroker messageBroker) {
    _repository = repository;
    _policy = policy;
    _messageBroker = messageBroker;
  }

  public async Task<Result> Handle (AcceptedAnswerChangedCommand command, CancellationToken cancellationToken) {
    var (oldAmount, newAmount) = _policy.FromAcceptedAnswer();

    var changes = await _repository.ApplyAcceptedAnswerAsync(
      command.SourceEventId, 
      command.SourceService, 
      command.CorrelationId,
      command.QuestionId,
      command.OldAnswerOwnerUserId, 
      oldAmount,
      command.NewAnswerOwnerUserId, 
      newAmount,
      command.OccurredAt, 
      command.Version, 
      cancellationToken);

    foreach (var change in changes)
      await _messageBroker.PublishAsync(change, cancellationToken);

    return Result.Success ();
  }

}