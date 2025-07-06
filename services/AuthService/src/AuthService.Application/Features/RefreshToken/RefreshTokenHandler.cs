using Ardalis.Result;

using AuthService.Application.Abstractions;
using AuthService.Application.Response;
using AuthService.Domain.Repositories;

namespace AuthService.Application.Features.RefreshToken;

public class RefreshTokenHandler : ICommandHandler<RefreshTokenResponse, RefreshTokenCommand> {

  private readonly IUserDataRepository _userDataRepository;
  private readonly ITokenService _tokenService;

  public RefreshTokenHandler (IUserDataRepository userDataRepository, ITokenService tokenService) {
    _userDataRepository = userDataRepository;
    _tokenService = tokenService;
  }

  public async Task<Result<RefreshTokenResponse>> Handle (RefreshTokenCommand command, CancellationToken cancellationToken) {
    var resultRefreshToken = await _userDataRepository.GetByRefreshTokenAsync(command.RefreshToken);
    if (!resultRefreshToken.IsSuccess)
      return Result<RefreshTokenResponse>.Invalid(
        new ValidationError("token.not.valid", "Неправильны токен.", "", ValidationSeverity.Error)
      );

    // (опционально) аннулировать старый токен в БД
    await _userDataRepository.RevokeRefreshTokenAsync(command.RefreshToken);

    var (access, refresh, exp) = _tokenService.GenerateTokens(resultRefreshToken.Value.Id, resultRefreshToken.Value.Email!);
    await _userDataRepository.AddRefreshTokenAsync(resultRefreshToken.Value.Id, refresh);
    await _userDataRepository.SaveChangesAsync();

    return Result<RefreshTokenResponse>
     .Success(new RefreshTokenResponse(access, refresh, exp));
  }

}