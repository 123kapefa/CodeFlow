using AuthService.Application.DTOs;

using FluentValidation;

namespace AuthService.Application.Features.LoginUser;

public class LoginUserValidator : AbstractValidator<LoginUserCommand> {

  public LoginUserValidator () {
    RuleFor (x => x.Email)
     .NotEmpty ().WithMessage ("Email обязателен")
     .EmailAddress ().WithMessage ("Неверный формат email");

    RuleFor (x => x.Password)
     .NotEmpty ().WithMessage ("Пароль обязателен");
  }

}