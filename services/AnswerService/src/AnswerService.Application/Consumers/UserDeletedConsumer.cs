using Abstractions.Commands;

using AnswerService.Application.Features.DeleteAnswer;
using AnswerService.Application.Features.DeleteAnswersByUserId;

using Contracts.Publishers.AuthService;

using MassTransit;

namespace AnswerService.Application.Consumers;

public class UserDeletedConsumer : IConsumer<UserDeleted> {

  private readonly ICommandHandler<DeleteAnswersByUserIdCommand> _handler;
  
  public UserDeletedConsumer (ICommandHandler<DeleteAnswersByUserIdCommand> handler) {
    _handler = handler;
  }

  public Task Consume (ConsumeContext<UserDeleted> context) {
    var message = context.Message;
    return _handler.Handle (
      new DeleteAnswersByUserIdCommand(message.UserId), 
      context.CancellationToken);
  }

}