using Abstractions.Commands;
using Contracts.Publishers.QuestionService;
using MassTransit;
using UserService.Application.Features.UpdateUserReputation;

namespace UserService.Application.Consumers;

public class QuestionVotedConsumer : IConsumer<QuestionVoted> {

    private readonly ICommandHandler<UpdateUserReputationCommand> _handler;

    public QuestionVotedConsumer( ICommandHandler<UpdateUserReputationCommand> handler ) {
        _handler = handler;
    }

    public Task Consume( ConsumeContext<QuestionVoted> context ) {
        var msg = context.Message;
        return _handler.Handle(
            new UpdateUserReputationCommand(msg.UserId, msg.ReputationValue), context.CancellationToken);
    }
}
