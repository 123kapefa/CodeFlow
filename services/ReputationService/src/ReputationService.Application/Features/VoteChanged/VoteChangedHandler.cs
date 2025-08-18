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
    var e = new Contracts.Publishers.VoteService.VoteChanged(command.SourceEventId, command.OccurredAt, command.EntityType, command.EntityId,
      command.EntityOwnerUserId, command.ActorUserId, command.OldKind, command.NewKind, command.CorrelationId);

    var entries = _policy.FromVoteChanged(e);
    if (entries.Count == 0) 
      return Result.NoContent();
    
    var reputationEntries = await _repository.AppendEntriesAsync(entries, cancellationToken);
    var reputationSummaries = await _repository.SetSummaryAsync(
      [command.ActorUserId, command.EntityOwnerUserId], 
      cancellationToken);
    
    foreach (var summary in reputationSummaries.Value) {
      await _messageBroker.PublishAsync(new UserReputationChanged ( 
        summary.UserId, summary.Total,summary.UpdatedAt), cancellationToken);
    }
    
    return Result.Success();
  }

}