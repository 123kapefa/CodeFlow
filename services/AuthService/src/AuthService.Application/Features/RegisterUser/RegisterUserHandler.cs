using Abstractions.Commands;

using Ardalis.Result;
using Ardalis.Result.FluentValidation;

using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;

using FluentValidation;

namespace AuthService.Application.Features.RegisterUser;

public class RegisterUserHandler : ICommandHandler<Guid, RegisterUserCommand> {
  
  private readonly IUserDataRepository _userDataRepository;
  private readonly IValidator<RegisterUserCommand> _validator;
  
  public RegisterUserHandler (
    IUserDataRepository userDataRepository
    , IValidator<RegisterUserCommand> validator) {
    _userDataRepository = userDataRepository;
    _validator = validator;
  }

  
  public async Task<Result<Guid>> Handle (RegisterUserCommand dataCommand, CancellationToken cancellationToken) {

    var validationResult = await _validator.ValidateAsync (dataCommand, cancellationToken);
    
    if (!validationResult.IsValid) {
      return Result<Guid>.Invalid(validationResult.AsErrors ());
    }

    var createResult = await _userDataRepository.CreateAsync (UserData.Create (dataCommand.Email), dataCommand.Password);


    if (!createResult.IsSuccess) {
      return createResult;
    }
    
    // TODO добавить отправку сообщение в другой сервис (UserService) для дальнейшей работы
    
    var userId = createResult.Value; 
    var response = new { UserId = userId, Email = dataCommand.Email, CreatedAt = DateTime.UtcNow };
    
    return Result<Guid>.Success (userId);
  }

}