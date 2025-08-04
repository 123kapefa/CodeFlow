using Abstractions.Commands;

using Contracts.Publishers.AnswerService;

using MassTransit;

using UserService.Application.Features.UpdateUserReputation;

namespace UserService.Application.Consumers;

public class AnswerAcceptedConsumer : IConsumer<AnswerAccepted> {

  private readonly ICommandHandler<UpdateUserReputationCommand> _handler;
  
  public AnswerAcceptedConsumer (ICommandHandler<UpdateUserReputationCommand> handler) {
    _handler = handler;
  }

  public Task Consume (ConsumeContext<AnswerAccepted> context) {
    var message = context.Message;
    return _handler.Handle(
      new UpdateUserReputationCommand(message.UserAnswerId, message.ReputationValue), 
      context.CancellationToken);
  }

}