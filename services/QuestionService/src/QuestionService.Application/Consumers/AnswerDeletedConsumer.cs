using Abstractions.Commands;

using Contracts.Publishers.AnswerService;

using MassTransit;

using QuestionService.Application.Features.ReduceQuestionAnswers;
using QuestionService.Application.Features.UpdateQuestionAnswers;

namespace QuestionService.Application.Consumers;

public class AnswerDeletedConsumer : IConsumer<AnswerDeleted> {

  private readonly ICommandHandler<ReduceQuestionAnswersCommand> _handler;
  
  public AnswerDeletedConsumer (ICommandHandler<ReduceQuestionAnswersCommand> handler) {
    _handler = handler;
  }

  public Task Consume (ConsumeContext<AnswerDeleted> context) {
    var message = context.Message;
    return _handler.Handle (
      new ReduceQuestionAnswersCommand (message.QuestionId), 
      context.CancellationToken);
  }

}