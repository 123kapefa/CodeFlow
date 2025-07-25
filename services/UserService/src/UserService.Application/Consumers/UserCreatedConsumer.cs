using Abstractions.Commands;

using Contracts.Publishers.AuthService;

using MassTransit;

using Microsoft.Extensions.Logging;

using UserService.Application.Features.CreateUserInfo;
using UserService.Domain.Entities;

namespace UserService.Application.Consumers;

public class UserRegisteredConsumer : IConsumer<UserRegistered> {

  private readonly ICommandHandler<CreateUserInfoCommand> _handler;

  public UserRegisteredConsumer (ICommandHandler<CreateUserInfoCommand> handler) {
    _handler = handler;
  }

  public async Task Consume (ConsumeContext<UserRegistered> context) {
    var msg = context.Message;
    await _handler.Handle (new CreateUserInfoCommand (msg.UserId, msg.Username), context.CancellationToken);
  }

}