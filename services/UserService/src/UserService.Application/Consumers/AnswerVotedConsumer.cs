using Abstractions.Commands;

using Contracts.Publishers.AnswerService;
using Contracts.Publishers.QuestionService;
using MassTransit;
using UserService.Application.Features.UpdateUserReputation;

namespace UserService.Application.Consumers;

public class AnswerVotedConsumer : IConsumer<AnswerVoted> {

    private readonly ICommandHandler<UpdateUserReputationCommand> _handler;
    
    public AnswerVotedConsumer( ICommandHandler<UpdateUserReputationCommand> handler ) {
        _handler = handler;
    }

    public Task Consume( ConsumeContext<AnswerVoted> context ) {
        var msg = context.Message;
        return _handler.Handle(
            new UpdateUserReputationCommand(msg.UserId, msg.ReputationValue), context.CancellationToken);
    }
}
