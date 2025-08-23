using Abstractions.Commands;

using Ardalis.Result;

using Contracts.Publishers.ReputationService;

using Messaging.Broker;

using ReputationService.Domain.Entities;
using ReputationService.Domain.Policies;
using ReputationService.Domain.Repositories;

namespace ReputationService.Application.Features.VoteChanged;

public class VoteChangedHandler : ICommandHandler<VoteChangedCommand> {

  private readonly IReputationRepository _repository;
  private readonly IReputationPolicy _policy;
  private readonly IMessageBroker _messageBroker;
  
  public VoteChangedHandler (IReputationRepository repository, IReputationPolicy policy, IMessageBroker messageBroker) {
    _repository = repository;
    _policy = policy;
    _messageBroker = messageBroker;
  }

  public async Task<Result> Handle (VoteChangedCommand command, CancellationToken cancellationToken) {
    var (delta, sourceType, ownerReason) = _policy.FromVote(command.SourceType, command.OldKind, command.NewKind);

    var changes = await _repository.ApplyVoteAsync(
      command.SourceEventId,
      command.ParentId,
      command.SourceId, 
      command.OwnerUserId, 
      command.SourceService, 
      sourceType,
      delta, 
      ownerReason,
      command.OccurredAt,
      command.Version,
      cancellationToken);

    foreach (var change in changes)
      await _messageBroker.PublishAsync(change, cancellationToken);
    await _repository.SaveAsync(cancellationToken);
    return Result.Success();
  }

}