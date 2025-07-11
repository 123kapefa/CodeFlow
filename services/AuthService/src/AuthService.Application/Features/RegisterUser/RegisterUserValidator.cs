using FluentValidation;

namespace AuthService.Application.Features.RegisterUser;

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand> {

  public RegisterUserValidator()
  {
    RuleFor(x => x.Email)
     .NotEmpty().WithMessage("Имя обязательно")
     .MinimumLength (6).WithMessage("Имя должно быть длинее 6 символов");
    
    RuleFor(x => x.Email)
     .NotEmpty().WithMessage("Email обязателен")
     .EmailAddress().WithMessage("Неверный формат email");

    RuleFor(x => x.Password)
     .NotEmpty().WithMessage("Пароль обязателен")
     .MinimumLength(6).WithMessage("Пароль минимум 6 символов");
  }

}