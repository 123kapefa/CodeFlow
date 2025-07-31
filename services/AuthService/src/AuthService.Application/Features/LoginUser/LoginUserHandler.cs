using Abstractions.Commands;

using Ardalis.Result;
using Ardalis.Result.FluentValidation;

using AuthService.Application.Abstractions;
using AuthService.Domain.Repositories;

using Contracts.AuthService.Responses;
using Contracts.Publishers.AuthService;
using FluentValidation;
using Messaging.Broker;

namespace AuthService.Application.Features.LoginUser;

public class LoginUserHandler : ICommandHandler<LoginResponse, LoginUserCommand> {

    private readonly IUserDataRepository _userDataRepository;
    private readonly IAuthTokenManager _authTokenManager;
    private readonly IValidator<LoginUserCommand> _validator;
    private readonly IMessageBroker _messageBroker;

    public LoginUserHandler( 
        IUserDataRepository userDataRepository, 
        IAuthTokenManager authTokenManager, 
        IValidator<LoginUserCommand> validator, 
        IMessageBroker messageBroker ) {

        _userDataRepository = userDataRepository;
        _authTokenManager = authTokenManager;
        _validator = validator;
        _messageBroker = messageBroker;
    }

    public async Task<Result<LoginResponse>> Handle( LoginUserCommand dataCommand, CancellationToken cancellationToken ) {
        var validationResult = await _validator.ValidateAsync(dataCommand, cancellationToken);
        if(!validationResult.IsValid) {
            return Result<LoginResponse>.Invalid(validationResult.AsErrors());
        }

        var result = await _userDataRepository.GetByEmailAsync(dataCommand.Email);
        if(!result.IsSuccess) {
            return Result<LoginResponse>.NotFound($"Неверный email или пароль");
        }

        var user = result.Value;

        var isValid = await _userDataRepository.CheckPasswordAsync(user, dataCommand.Password);
        if(!isValid.IsSuccess) {
            return Result<LoginResponse>.Error("Неверный email или пароль");
        }

        var tokensResult = await _authTokenManager.CreateTokensAsync(user.Id, user.Email!, TimeSpan.FromDays(7));

        if(!tokensResult.IsSuccess) {
            return Result<LoginResponse>.Error(new ErrorList(tokensResult.Errors));
        }

        var (access, refresh, expiresInSeconds) = tokensResult.Value;

        var response = new LoginResponse(access, refresh, expiresInSeconds);

        await _messageBroker.PublishAsync(new UserLogged(user.Id), cancellationToken);
        await _userDataRepository.SaveChangesAsync(cancellationToken);

        return Result<LoginResponse>.Success(response);
    }

}