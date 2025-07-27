using Abstractions.Commands;
using Contracts.Publishers.AuthService;
using MassTransit;
using UserService.Application.Features.UpdateUserVisit;

namespace UserService.Application.Consumers;

public class UserLoggedConsumer : IConsumer<UserLogged> {

    private readonly ICommandHandler<UpdateUserVisitCommand> _handler;

    public UserLoggedConsumer( ICommandHandler<UpdateUserVisitCommand> handler ) {
        _handler = handler;
    }

    public Task Consume( ConsumeContext<UserLogged> context ) {
        UserLogged msg = context.Message;        
        return _handler.Handle(new UpdateUserVisitCommand(msg.UserId), context.CancellationToken);
    }
}
