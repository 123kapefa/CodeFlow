using Ardalis.Result;

namespace Abstractions.Commands;

public interface ICommandHandler<in TCommand> where TCommand : ICommand {  
  
  Task<Result> Handle (TCommand command, CancellationToken cancellationToken);  
  
}