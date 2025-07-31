using Abstractions.Commands;

using Contracts.Publishers.AnswerService;

using MassTransit;

using TagService.Application.Features.ParticipationTags.DeleteAnswerTags;

namespace TagService.Application.Consumers;

public class AnswerDeletedConsumer : IConsumer<AnswerDeleted> {

  private readonly ICommandHandler<DeleteAnswerTagsCommand> _handler;
  
  public AnswerDeletedConsumer (ICommandHandler<DeleteAnswerTagsCommand> handler) {
    _handler = handler;
  }

  public Task Consume (ConsumeContext<AnswerDeleted> context) {
    var message = context.Message;
    return _handler.Handle (
      new DeleteAnswerTagsCommand (
        message.UserId, 
        message.QuestionId,
        message.TagIds), 
      context.CancellationToken);
  }

}