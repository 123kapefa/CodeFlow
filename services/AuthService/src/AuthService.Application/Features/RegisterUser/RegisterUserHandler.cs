using Ardalis.Result;
using Ardalis.Result.FluentValidation;

using AuthService.Application.Abstractions;
using AuthService.Application.DTOs;
using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;

using FluentValidation;

namespace AuthService.Application.Features.RegisterUser;

public class RegisterUserHandler : ICommandHandler<Guid, RegisterUserCommand> {
  
  private readonly IUserDataRepository _userDataRepository;
  private readonly IValidator<RegisterUserDto> _validator;
  
  public RegisterUserHandler (
    IUserDataRepository userDataRepository
    , IValidator<RegisterUserDto> validator) {
    _userDataRepository = userDataRepository;
    _validator = validator;
  }

  
  public async Task<Result<Guid>> Handle (RegisterUserCommand command, CancellationToken cancellationToken) {

    var validationResult = await _validator.ValidateAsync (command.Request, cancellationToken);
    
    if (!validationResult.IsValid) {
      return Result<Guid>.Invalid(validationResult.AsErrors ());
    }

    var createResult = await _userDataRepository.CreateAsync (UserData.Create (command.Request.Email), command.Request.Password);

    if (!createResult.IsSuccess) {
      return createResult;
    }
    
    // TODO добавить отправку сообщение в другой сервис (UserService) для дальнейшей работы
    
    var userId = createResult.Value; 
    var response = new { UserId = userId, Email = command.Request.Email, CreatedAt = DateTime.UtcNow };
    
    return Result<Guid>.Success (userId);
  }

}