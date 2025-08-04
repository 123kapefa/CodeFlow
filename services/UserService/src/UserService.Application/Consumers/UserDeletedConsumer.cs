using Abstractions.Commands;

using Contracts.Publishers.AuthService;

using MassTransit;

using UserService.Application.Features.DeleteUser;

namespace UserService.Application.Consumers;

public class UserDeletedConsumer : IConsumer<UserDeleted> {

  private readonly ICommandHandler<DeleteUserCommand> _handler;
  
  public UserDeletedConsumer (ICommandHandler<DeleteUserCommand> handler) {
    _handler = handler;
  }

  public Task Consume (ConsumeContext<UserDeleted> context) {
    var message = context.Message;
    return _handler.Handle(
      new DeleteUserCommand(message.UserId), 
      context.CancellationToken);
  }

}