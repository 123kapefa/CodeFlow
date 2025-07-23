using Abstractions.Commands;

using Ardalis.Result;

using AuthService.Domain.Repositories;

using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.RemoveUser;

public class RemoveUserHandler : ICommandHandler<RemoveUserCommand> {

  private readonly IUserDataRepository _userDataRepository;
  private readonly ILogger<RemoveUserHandler> _logger;
  
  public RemoveUserHandler (IUserDataRepository userDataRepository, ILogger<RemoveUserHandler> logger) {
    _userDataRepository = userDataRepository;
    _logger = logger;
  }

  public async Task<Result> Handle (RemoveUserCommand command, CancellationToken cancellationToken) {
    var user = await _userDataRepository.GetByIdAsync(command.UserId);
    if (!user.IsSuccess) 
      return Result.Error(new ErrorList(user.Errors));

    await _userDataRepository.DeleteAsync(user.Value);
    return Result.Success();
  }

}