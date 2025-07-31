using Abstractions.Commands;

using Contracts.Publishers.AnswerService;
using MassTransit;

using TagService.Application.Features.ParticipationTags.UpdateParticipationAnswer;

namespace TagService.Application.Consumers;

public class AnswerCreatedConsumer : IConsumer<AnswerCreated> {

  private readonly ICommandHandler<UpdateParticipationAnswerCommand> _handler;
  
  public AnswerCreatedConsumer (ICommandHandler<UpdateParticipationAnswerCommand> handler) {
    _handler = handler;
  }

  public Task Consume (ConsumeContext<AnswerCreated> context) {
    var message = context.Message;
    return _handler.Handle (
      new UpdateParticipationAnswerCommand (
        message.Tags, 
        message.UserId, 
        message.QuestionId), 
      context.CancellationToken);
  }

}