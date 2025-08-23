using Abstractions.Commands;

using AnswerService.Application.Features.UpdateAnswerAccept;
using AnswerService.Application.Features.UpdateAnswerVote;

using Contracts.Publishers.AnswerService;
using Contracts.Publishers.VoteService;

using MassTransit;

namespace AnswerService.Application.Consumers;

public class AnswerVotedConsumer : IConsumer<AnswerVoted> {

  private readonly ICommandHandler<UpdateAnswerVoteCommand> _handler;
  
  public AnswerVotedConsumer (ICommandHandler<UpdateAnswerVoteCommand> handler) {
    _handler = handler;
  }

  public Task Consume (ConsumeContext<AnswerVoted> context) {
    var message = context.Message;
    return _handler.Handle (
      new UpdateAnswerVoteCommand(message.AnswerId, message.AuthorUserId, message.ValueNewKind), 
      context.CancellationToken);
  }

}