using Abstractions.Commands;

using Contracts.Publishers.VoteService;

using MassTransit;

using ReputationService.Application.Features.VoteChanged;

namespace ReputationService.Application.Consumers;

public class QuestionVotedConsumer : IConsumer<QuestionVoted> {

  private readonly ICommandHandler<VoteChangedCommand> _handler;
  
  public QuestionVotedConsumer (ICommandHandler<VoteChangedCommand> handler) {
    _handler = handler;
  }

  public Task Consume (ConsumeContext<QuestionVoted> context) {
    
    Console.WriteLine($"[QVC] >>> {context.Message.SourceEventId} {context.Message.QuestionId}");
    
    var message = context.Message;
    var command = new VoteChangedCommand(
      SourceEventId: message.SourceEventId,
      ParentId: message.ParentId,
      SourceId: message.QuestionId,
      OwnerUserId: message.OwnerUserId,
      AuthorUserId: message.AuthorUserId,
      SourceService: "VoteService",
      SourceType: VotableSourceType.Question, 
      OldKind: message.ValueOldKind,
      NewKind: message.ValueNewKind,
      Version: message.Version,
      OccurredAt: message.OccurredAt
    );
    Console.WriteLine($"[QVC] <<< done {context.Message.SourceEventId}");
    return _handler.Handle(command, context.CancellationToken);
  }

}