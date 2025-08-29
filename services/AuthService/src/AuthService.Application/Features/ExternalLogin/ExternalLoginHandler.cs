using System.Security.Claims;

using Abstractions.Commands;

using Ardalis.Result;

using AuthService.Application.Abstractions;
using AuthService.Domain.Entities;

using Contracts.Responses.AuthService;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.ExternalLogin;

public class ExternalLoginHandler : ICommandHandler<LoginResponse, ExternalLoginCommand>
{
    private readonly UserManager<UserData> _userManager;
    private readonly IAuthTokenManager _authTokenManager;
    private readonly ILogger<ExternalLoginHandler> _logger;

    public ExternalLoginHandler(
        UserManager<UserData> userManager,
        IAuthTokenManager authTokenManager,
        ILogger<ExternalLoginHandler> logger)
    {
        _userManager = userManager;
        _authTokenManager = authTokenManager;
        _logger = logger;
    }

    public async Task<Result<LoginResponse>> Handle(ExternalLoginCommand command, CancellationToken cancellationToken)
    {
        var principal = command.Principal;

        var email = principal.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrWhiteSpace(email))
        {
            _logger.LogWarning("Email claim not found.");
            return Result<LoginResponse>.Unauthorized();
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = UserData.Create(email);
            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded) {
                var errors = result.Errors.Select(e => e.Description);
                _logger.LogError("Ошибка при создании пользователя: {Errors}", string.Join("; ", errors));
                return Result<LoginResponse>.Error(new ErrorList(errors));
            }

            _logger.LogInformation("Создан новый пользователь через внешнюю аутентификацию: {Email}", email);
        }

        var tokens = await _authTokenManager.CreateTokensAsync(user.Id, user.Email!, TimeSpan.FromDays(7));
        if (!tokens.IsSuccess)
            return Result<LoginResponse>.Error(new ErrorList(tokens.Errors));

        return Result<LoginResponse>.Success(new LoginResponse(
            tokens.Value.AccessToken,
            tokens.Value.RefreshToken,
            tokens.Value.ExpiresInSeconds
        ));
    }
}


// public class ExternalLoginHandler : ICommandHandler<LoginResponse, GoogleLoginCommand>
//   , ICommandHandler<LoginResponse, GithubLoginCommand> {
//
//   private readonly IGoogleAuthService _googleAuthService;
//   private readonly IGithubAuthService _githubAuthService;
//   private readonly IUserDataRepository _userDataRepository;
//   private readonly IAuthTokenManager _authTokenManager;
//   private readonly ILogger<ExternalLoginHandler> _logger;
//   
//   public ExternalLoginHandler (
//     IGoogleAuthService googleAuthService
//     , IGithubAuthService githubAuthService
//     , IUserDataRepository userRepository
//     , ILogger<ExternalLoginHandler> logger
//     , IAuthTokenManager authTokenManager) {
//     _googleAuthService = googleAuthService;
//     _githubAuthService = githubAuthService;
//     _userDataRepository = userRepository;
//     _authTokenManager = authTokenManager;
//     _logger = logger;
//   }
//
//   public async Task<Result<LoginResponse>> Handle (GoogleLoginCommand command, CancellationToken cancellationToken) {
//     _logger.LogInformation ("Google login, validating IdToken");
//     
//     // TODO Нужно реализовать вход с помощью Google
//     var payload = await _googleAuthService.ValidateIdTokenAsync (command.IdToken);
//     if (!payload.EmailVerified)
//       return Result<LoginResponse>.Error ("Email не подтверждён Google.");
//     
//     var email = payload.Email;
//
//     var userRes = await _userDataRepository.GetByEmailAsync (email);
//     UserData user;
//     if (!userRes.IsSuccess) {
//       user = UserData.Create (email);
//       await _userDataRepository.CreateAsync (user, password: null);
//       _logger.LogInformation ("Создан новый пользователь {UserId} через Google", user.Id);
//     } else {
//       user = userRes.Value;
//     }
//     
//     var tokensResult = await _authTokenManager.CreateTokensAsync(user.Id, user.Email!, TimeSpan.FromDays (7));
//
//     if (!tokensResult.IsSuccess) {
//       return Result<LoginResponse>.Error(new ErrorList (tokensResult.Errors));
//     }
//     
//     return Result<LoginResponse>.Success (new LoginResponse(
//       tokensResult.Value.AccessToken, 
//       tokensResult.Value.RefreshToken,
//       tokensResult.Value.ExpiresInSeconds));
//   }
//
//   public Task<Result<LoginResponse>> Handle (GithubLoginCommand command, CancellationToken cancellationToken) {
//     throw new NotImplementedException ();
//   }
//
// }

