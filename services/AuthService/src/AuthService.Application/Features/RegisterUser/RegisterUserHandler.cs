using Ardalis.Result;

using AuthService.Application.Abstractions;
using AuthService.Application.DTOs;
using AuthService.Application.Extensions;
using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;

using FluentValidation;

using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.Register;

public class RegisterUserHandler : ICommandHandler<Guid, RegisterUserCommand> {
  
  private readonly IUserDataRepository _userDataRepository;
  private readonly IValidator<RegisterDto> _validator;

  public RegisterUserHandler (
    IUserDataRepository userDataRepository
    , IValidator<RegisterDto> validator) {
    _userDataRepository = userDataRepository;
    _validator = validator;
  }

  public async Task<Result<Guid>> Handle (RegisterUserCommand command, CancellationToken cancellationToken) {

    var dto = new RegisterDto (command.Email, command.Password);

    var validationResult = await _validator.ValidateAsync (dto, cancellationToken);
    
    if (!validationResult.IsValid) {
      return validationResult.ToInvalidResult<Guid> ("user.not.valid");
    }

    var createResult = await _userDataRepository.CreateAsync (UserData.Create (command.Email), command.Password);

    if (!createResult.IsSuccess) {
      return createResult;
    }

    var userId = createResult.Value; 
    var response = new { UserId = userId, Email = command.Email, CreatedAt = DateTime.UtcNow };
    
    return Result<Guid>.Success (userId);
  }

}