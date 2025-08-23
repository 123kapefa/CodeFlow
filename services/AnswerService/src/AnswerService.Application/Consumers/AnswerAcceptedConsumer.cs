using Abstractions.Commands;

using AnswerService.Application.Features.UpdateAnswerAccept;

using Contracts.Publishers.AnswerService;

using MassTransit;

namespace AnswerService.Application.Consumers;

public class AnswerAcceptedConsumer : IConsumer<AnswerAccepted> {

  private readonly ICommandHandler<UpdateAnswerAcceptCommand> _handler;
  
  public AnswerAcceptedConsumer (ICommandHandler<UpdateAnswerAcceptCommand> handler) {
    _handler = handler;
  }

  public Task Consume (ConsumeContext<AnswerAccepted> context) {
    var message = context.Message;
    return _handler.Handle (
      new UpdateAnswerAcceptCommand((Guid)message.NewAnswerId!, message.ParentId), 
      context.CancellationToken);
  }

}