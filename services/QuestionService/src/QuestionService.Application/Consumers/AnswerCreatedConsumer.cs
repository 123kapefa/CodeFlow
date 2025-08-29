using Abstractions.Commands;

using Contracts.Publishers.AnswerService;

using MassTransit;

using QuestionService.Application.Features.UpdateQuestionAnswers;

namespace QuestionService.Application.Consumers;

public class AnswerCreatedConsumer : IConsumer<AnswerCreated> {

  private readonly ICommandHandler<UpdateQuestionAnswersCommand> _handler;
  
  public AnswerCreatedConsumer (ICommandHandler<UpdateQuestionAnswersCommand> handler) {
    _handler = handler;
  }

  public Task Consume (ConsumeContext<AnswerCreated> context) {
    var message = context.Message;
    return _handler.Handle (
      new UpdateQuestionAnswersCommand (message.QuestionId), 
      context.CancellationToken);
  }

}