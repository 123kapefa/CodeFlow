using Abstractions.Commands;

using Ardalis.Result;

using AuthService.Application.Abstractions;
using AuthService.Domain.Repositories;

using Contracts.Responses.AuthService;

namespace AuthService.Application.Features.RefreshToken;

public class RefreshTokenHandler : ICommandHandler<RefreshTokenResponse, RefreshTokenCommand> {

  private readonly IUserDataRepository _userDataRepository;
  private readonly IAuthTokenManager _authTokenManager;

  public RefreshTokenHandler (
    IUserDataRepository userDataRepository, 
    IAuthTokenManager authTokenManager) {
    _userDataRepository = userDataRepository;
    _authTokenManager = authTokenManager;
  }

  public async Task<Result<RefreshTokenResponse>> Handle (RefreshTokenCommand command, CancellationToken cancellationToken) {
    var userDataResult = await _userDataRepository.GetByRefreshTokenAsync(command.RefreshToken);
    if (!userDataResult.IsSuccess)
      return Result<RefreshTokenResponse>.Invalid(
        new ValidationError("token.not.valid", "Неправильны токен.", "", ValidationSeverity.Error)
      );
    
    await _authTokenManager.RevokeRefreshAsync(command.RefreshToken);

    var tokensResult = await _authTokenManager.RefreshTokensAsync(command.RefreshToken, TimeSpan.FromDays (7));

    if (!tokensResult.IsSuccess) {
      return Result<RefreshTokenResponse>.Error(new ErrorList (tokensResult.Errors));
    }

    var (access, refresh, expiresInSeconds) = tokensResult.Value;

    var response = new RefreshTokenResponse(
      AccessToken:    access,
      RefreshToken:   refresh,
      ExpiresInSeconds: expiresInSeconds);
    
    return Result<RefreshTokenResponse>
     .Success(response);
  }

}