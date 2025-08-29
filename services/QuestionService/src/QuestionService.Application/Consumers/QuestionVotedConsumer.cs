using Abstractions.Commands;

using Contracts.Publishers.QuestionService;
using Contracts.Publishers.VoteService;

using MassTransit;

using QuestionService.Application.Features.UpdateQuestionVote;

namespace QuestionService.Application.Consumers;

public class QuestionVotedConsumer : IConsumer<QuestionVoted> {

  private readonly ICommandHandler<UpdateQuestionVoteCommand> _handler;
  
  public QuestionVotedConsumer (ICommandHandler<UpdateQuestionVoteCommand> handler) {
    _handler = handler;
  }

  public Task Consume (ConsumeContext<QuestionVoted> context) {
    var message = context.Message;
    return _handler.Handle (
      new UpdateQuestionVoteCommand(message.QuestionId, message.AuthorUserId, message.ValueNewKind), 
      context.CancellationToken);
  }

}