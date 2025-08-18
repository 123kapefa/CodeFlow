using Abstractions.Commands;

using Contracts.Publishers.VoteService;

using MassTransit;

using ReputationService.Application.Features.VoteChanged;

namespace ReputationService.Application.Consumers;

public class VoteChangedConsumer : IConsumer<VoteChanged> {

  private readonly ICommandHandler<VoteChangedCommand> _handler;
  
  public VoteChangedConsumer (ICommandHandler<VoteChangedCommand> handler) {
    _handler = handler;
  }

  public Task Consume (ConsumeContext<VoteChanged> context) {
    var message = context.Message;
    var command = new VoteChangedCommand(
      SourceEventId: message.EventId, 
      SourceService: "VoteService", 
      CorrelationId: message.CorrelationId, 
      OccurredAt: message.OccurredAt,
      EntityType: message.EntityType, 
      EntityId: message.EntityId, 
      EntityOwnerUserId: message.EntityOwnerUserId, 
      ActorUserId: message.ActorUserId, 
      NewKind: message.NewKind, 
      Version: message.Version);
    return _handler.Handle(command, context.CancellationToken);
  }

}