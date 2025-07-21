using Ardalis.Result;

namespace Abstractions.Commands;

public interface ICommandHandler<TResponse, in TCommand> where TCommand : ICommand {  
  
  Task<Result<TResponse>> Handle (TCommand command, CancellationToken cancellationToken);  
  
}  