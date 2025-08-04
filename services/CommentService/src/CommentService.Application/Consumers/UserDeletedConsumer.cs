using Abstractions.Commands;

using CommentService.Application.Features.DeleteAllUserComments;

using Contracts.Publishers.AuthService;

using MassTransit;

namespace CommentService.Application.Consumers;

public class UserDeletedConsumer : IConsumer<UserDeleted> {

  private readonly ICommandHandler<DeleteAllUserCommentsCommand> _handler;
  
  public UserDeletedConsumer (ICommandHandler<DeleteAllUserCommentsCommand> handler) {
    _handler = handler;
  }

  public Task Consume (ConsumeContext<UserDeleted> context) {
    var message = context.Message;
    return _handler.Handle(
      new DeleteAllUserCommentsCommand(message.UserId), 
      context.CancellationToken);
  }

}