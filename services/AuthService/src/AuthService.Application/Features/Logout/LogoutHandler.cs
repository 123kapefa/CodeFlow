using Ardalis.Result;

using AuthService.Application.Abstractions;
using AuthService.Domain.Repositories;

using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.Logout;

public class LogoutHandler : ICommandHandler<bool, LogoutCommand> {

  private readonly IUserDataRepository _userDataRepository;
  private readonly ILogger<LogoutHandler> _logger;

  public LogoutHandler (IUserDataRepository userDataRepository, ILogger<LogoutHandler> logger) {
    _userDataRepository = userDataRepository;
    _logger = logger;
  }

  public async Task<Result<bool>> Handle (LogoutCommand command, CancellationToken cancellationToken) {
    await _userDataRepository.RevokeRefreshTokenAsync(command.RefreshToken);
    await _userDataRepository.SaveChangesAsync();
    return Result.Success(true);

  }

}