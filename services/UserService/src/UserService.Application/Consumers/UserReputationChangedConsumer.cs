using Abstractions.Commands;

using Contracts.Publishers.ReputationService;

using MassTransit;

using UserService.Application.Features.UpdateUserReputation;

namespace UserService.Application.Consumers;

public class UserReputationChangedConsumer : IConsumer<UserReputationChanged> {

  private readonly ICommandHandler<UpdateUserReputationCommand> _handler;
  
  public UserReputationChangedConsumer (ICommandHandler<UpdateUserReputationCommand> handler) {
    _handler = handler;
  }

  public Task Consume (ConsumeContext<UserReputationChanged> context) {
    var message = context.Message;
    return _handler.Handle(
      new UpdateUserReputationCommand(
        message.UserId,
        message.NewReputation,
        message.OccurredAt), 
      context.CancellationToken);
  }

}