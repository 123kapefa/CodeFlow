using Abstractions.Commands;

using Ardalis.Result;

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
    // var result = await _repository.AppendEntryAsync (
    //   ReputationEntry.Create (
    //     command.Userid, command.SourceId, command.SourceType, command.Delta, command.ReasonCode,
    //     command.OccurredAt, command.SourceEventId, command.SourceService, command.CorrelationId), cancellationToken);


    return Result.Success ();
  }

}