using Abstractions.Commands;

using CommentService.Application.Features.DeleteAllUserComments;
using CommentService.Application.Features.DeleteByAnswerId;

using Contracts.Publishers.AnswerService;

using MassTransit;

namespace CommentService.Application.Consumers;

public class AnswerDeletedConsumer : IConsumer<AnswerDeleted> {

  private readonly ICommandHandler<DeleteByAnswerIdCommand> _handler;
  
  public AnswerDeletedConsumer (ICommandHandler<DeleteByAnswerIdCommand> handler) {
    _handler = handler;
  }

  public Task Consume (ConsumeContext<AnswerDeleted> context) {
    var message = context.Message;
    return _handler.Handle(
      new DeleteByAnswerIdCommand(message.AnswerId), 
      context.CancellationToken);
  }
}