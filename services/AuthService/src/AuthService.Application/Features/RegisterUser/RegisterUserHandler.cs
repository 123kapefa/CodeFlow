using Abstractions.Commands;

using Ardalis.Result;
using Ardalis.Result.FluentValidation;

using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;

using Contracts.Publishers.AuthService;

using FluentValidation;

using Messaging.Broker;

namespace AuthService.Application.Features.RegisterUser;

public class RegisterUserHandler : ICommandHandler<Guid, RegisterUserCommand> {
  
  private readonly IUserDataRepository _userDataRepository;
  private readonly IValidator<RegisterUserCommand> _validator;
  private readonly IMessageBroker _messageBroker;
  
  public RegisterUserHandler (
    IUserDataRepository userDataRepository
    , IValidator<RegisterUserCommand> validator
    , IMessageBroker messageBroker) {
    _userDataRepository = userDataRepository;
    _validator = validator;
    _messageBroker = messageBroker;
  }

  
  public async Task<Result<Guid>> Handle (RegisterUserCommand dataCommand, CancellationToken cancellationToken) {

    var validationResult = await _validator.ValidateAsync (dataCommand, cancellationToken);
    
    if (!validationResult.IsValid) {
      return Result<Guid>.Invalid(validationResult.AsErrors ());
    }

    var createResult = await _userDataRepository.CreateAsync (UserData.Create (dataCommand.Email), dataCommand.Password);

    if (!createResult.IsSuccess) {
      return Result<Guid>.Error (new ErrorList (createResult.Errors));
    }
    
    await _messageBroker.PublishAsync (new UserRegistered (createResult.Value, dataCommand.Username), cancellationToken);
    await _userDataRepository.SaveChangesAsync ();
    
    var userId = createResult.Value; 
    var response = new { UserId = userId, Email = dataCommand.Email, CreatedAt = DateTime.UtcNow };
    
    return Result<Guid>.Success (userId);
  }

}