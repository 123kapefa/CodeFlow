using Ardalis.Result;
using Ardalis.Result.FluentValidation;

using AuthService.Application.Abstractions;
using AuthService.Application.DTOs;
using AuthService.Application.Response;
using AuthService.Domain.Repositories;

using FluentValidation;

namespace AuthService.Application.Features.LoginUser;

public class LoginUserHandler : ICommandHandler<LoginResponse, LoginUserCommand> {

  private readonly IUserDataRepository _userDataRepository;
  private readonly ITokenService _tokenService;
  private readonly IValidator<LoginUserCommand> _validator;

  public LoginUserHandler (
    IUserDataRepository userDataRepository, 
    IValidator<LoginUserCommand> validator, 
    ITokenService tokenService) {
    _userDataRepository = userDataRepository;
    _validator = validator;
    _tokenService = tokenService;
  }

  public async Task<Result<LoginResponse>> Handle (LoginUserCommand command, CancellationToken cancellationToken) {
    var validationResult = await _validator.ValidateAsync (command, cancellationToken);
    if (!validationResult.IsValid) {
      return Result<LoginResponse>.Invalid (validationResult.AsErrors());
    }

    var result = await _userDataRepository.GetByEmailAsync (command.Email);
    if (!result.IsSuccess) {
      return Result<LoginResponse>.NotFound ($"Неверный email или пароль");
    }

    var user = result.Value;

    var isValid = await _userDataRepository.CheckPasswordAsync (user, command.Password);
    if (!isValid.Value) {
      return Result<LoginResponse>.Error (new ErrorList (new[] { "Неверный email или пароль" }));
    }

    var (access, refresh, expires) = _tokenService.GenerateTokens (user.Id, user.Email!);

    var response = new LoginResponse (access, refresh, expires);
    
    return Result<LoginResponse>.Success (response);
  }

}