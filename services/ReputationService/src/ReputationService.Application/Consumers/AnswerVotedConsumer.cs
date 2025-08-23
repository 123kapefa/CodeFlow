using Abstractions.Commands;

using Contracts.Publishers.VoteService;

using MassTransit;

using ReputationService.Application.Features.VoteChanged;

namespace ReputationService.Application.Consumers;

public class AnswerVotedConsumer : IConsumer<AnswerVoted> {

  private readonly ICommandHandler<VoteChangedCommand> _handler;
  
  public AnswerVotedConsumer (ICommandHandler<VoteChangedCommand> handler) {
    _handler = handler;
  }

  public Task Consume (ConsumeContext<AnswerVoted> context) {
    var message = context.Message;
    var command = new VoteChangedCommand(
      SourceEventId: message.SourceEventId,
      ParentId: message.ParentId,
      SourceId: message.AnswerId,
      OwnerUserId: message.OwnerUserId,
      AuthorUserId: message.AuthorUserId,
      SourceService: "VoteService",
      SourceType: VotableSourceType.Answer, 
      OldKind: message.ValueOldKind,
      NewKind: message.ValueNewKind,
      Version: message.Version,
      OccurredAt: message.OccurredAt
    );
    return _handler.Handle(command, context.CancellationToken);
  }

}