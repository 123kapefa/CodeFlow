using Abstractions.Commands;

using Ardalis.Result;

using AuthService.Domain.Repositories;

using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.LogoutUser;

public class LogoutUserHandler : ICommandHandler<LogoutUserCommand> {

  private readonly IRefreshTokenRepository _refreshTokenRepository;
  private readonly ILogger<LogoutUserHandler> _logger;

  public LogoutUserHandler (IRefreshTokenRepository refreshTokenRepository, ILogger<LogoutUserHandler> logger) {
    _refreshTokenRepository = refreshTokenRepository;
    _logger = logger;
  }

  public async Task<Result> Handle (LogoutUserCommand userCommand, CancellationToken cancellationToken) {
    await _refreshTokenRepository.RevokeAsync(userCommand.RefreshToken);
    return Result.Success();
  }

}