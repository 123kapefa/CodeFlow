using Abstractions.Commands;

using Contracts.Publishers.AnswerService;

using MassTransit;

using ReputationService.Application.Features.AcceptedAnswerChanged;

namespace ReputationService.Application.Consumers;

public sealed class AcceptedAnswerChangedConsumer : IConsumer<AnswerAccepted>
{
  private readonly ICommandHandler<AcceptedAnswerChangedCommand> _handler;
  public AcceptedAnswerChangedConsumer(ICommandHandler<AcceptedAnswerChangedCommand> handler) => _handler = handler;

  public Task Consume(ConsumeContext<AnswerAccepted> context)
  {
    var message = context.Message;
    var command = new AcceptedAnswerChangedCommand (
      SourceEventId: message.EventId, 
      ParentId: message.ParentId,
      OldAnswerId: message.OldAnswerId,
      OldAnswerOwnerUserId: message.OldAnswerOwnerUserId, 
      NewAnswerId: message.NewAnswerId,
      NewAnswerOwnerUserId: message.NewAnswerOwnerUserId,
      SourceService: "AnswerService", 
      Version: message.Version,
      OccurredAt: message.OccurredAt);
    return _handler.Handle(command, context.CancellationToken);
  }
}
