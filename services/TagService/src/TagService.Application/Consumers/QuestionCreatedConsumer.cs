using Abstractions.Commands;

using Contracts.Publishers.QuestionService;

using MassTransit;

using TagService.Application.Features.ParticipationTags.CreateOrUpdateParticipationTags;

namespace TagService.Application.Consumers;

public class QuestionCreatedConsumer : IConsumer<QuestionCreated> {

  private readonly ICommandHandler<CreateOrUpdateParticipationTagsCommand> _handler;
  
  public QuestionCreatedConsumer (ICommandHandler<CreateOrUpdateParticipationTagsCommand> handler) {
    _handler = handler;
  }

  public Task Consume (ConsumeContext<QuestionCreated> context) {
    var message = context.Message;
    return _handler.Handle (
      new CreateOrUpdateParticipationTagsCommand (message.QuestionId, message.UserId, message.Tags)
      , context.CancellationToken);
  }

}