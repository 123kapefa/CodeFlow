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
    var (oldDelta, newDelta, reason) = _policy.FromAcceptedAnswerChange();

    var changes = await _repository.ApplyAcceptedAnswerAsync(
      command.SourceEventId,
      command.ParentId,
      command.SourceService,
      command.OldAnswerId,
      command.OldAnswerOwnerUserId,
      oldDelta,
      command.NewAnswerId,
      command.NewAnswerOwnerUserId,
      newDelta,
      reason,
      command.OccurredAt,
      cancellationToken);

        foreach(var change in changes)            
            await _messageBroker.PublishAsync(change, cancellationToken);      

        await _repository.SaveAsync(cancellationToken);

        return Result.Success ();
  }

}