using AuthService.Application.DTOs;

using FluentValidation;

namespace AuthService.Application.Features.RegisterUser;

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand> {

  public RegisterUserValidator()
  {
    RuleFor(x => x.Email)
     .NotEmpty().WithMessage("Email обязателен")
     .EmailAddress().WithMessage("Неверный формат email");

    RuleFor(x => x.Password)
     .NotEmpty().WithMessage("Пароль обязателен")
     .MinimumLength(6).WithMessage("Пароль минимум 6 символов");
  }

}