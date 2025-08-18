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
    var command = new AcceptedAnswerChangedCommand(
      SourceEventId: message.EventId, 
      SourceService: "AnswerService", 
      CorrelationId: message.CorrelationId, 
      OccurredAt: message.OccurredAt,
      QuestionId: message.QuestionId, 
      OldAnswerOwnerUserId: message.OldAnswerOwnerUserId, 
      NewAnswerOwnerUserId: message.NewAnswerOwnerUserId, 
      Version: message.Version);
    return _handler.Handle(command, context.CancellationToken);
  }
}
