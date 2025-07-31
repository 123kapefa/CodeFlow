using Abstractions.Commands;

using Contracts.Publishers.AuthService;

using MassTransit;

using TagService.Application.Features.ParticipationTags.DeleteUserTags;

namespace TagService.Application.Consumers;

public class UserDeletedConsumer : IConsumer<UserDeleted> {

  private readonly ICommandHandler<DeleteUserTagsCommand> _handler;
  
  public UserDeletedConsumer (ICommandHandler<DeleteUserTagsCommand> handler) {
    _handler = handler;
  }

  public Task Consume (ConsumeContext<UserDeleted> context) {
    var message = context.Message;
    return _handler.Handle (
      new DeleteUserTagsCommand (
        message.UserId), 
      context.CancellationToken);
  }

}