using Abstractions.Commands;

using Ardalis.Result;

using AuthService.Domain.Repositories;

using Contracts.Publishers.AuthService;

using Messaging.Broker;

using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.RemoveUser;

public class RemoveUserHandler : ICommandHandler<RemoveUserCommand> {

  private readonly IMessageBroker _messageBroker;
  private readonly IUserDataRepository _userDataRepository;
  private readonly ILogger<RemoveUserHandler> _logger;

  public RemoveUserHandler (
    IUserDataRepository userDataRepository
    , ILogger<RemoveUserHandler> logger
    , IMessageBroker messageBroker) {
    _userDataRepository = userDataRepository;
    _logger = logger;
    _messageBroker = messageBroker;
  }

  public async Task<Result> Handle (RemoveUserCommand command, CancellationToken cancellationToken) {
    var user = await _userDataRepository.GetByIdAsync (command.UserId);
    if (!user.IsSuccess)
      return Result.Error (new ErrorList (user.Errors));

    await _userDataRepository.DeleteAsync (user.Value);
    await _messageBroker.PublishAsync (new UserDeleted (user.Value.Id), cancellationToken);
    await _userDataRepository.SaveChangesAsync (cancellationToken);
    
    return Result.Success ();
  }

}