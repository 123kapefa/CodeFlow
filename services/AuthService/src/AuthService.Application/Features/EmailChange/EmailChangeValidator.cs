using FluentValidation;

namespace AuthService.Application.Features.RequestEmailChange;

public class EmailChangeValidator : AbstractValidator<EmailChangeCommand> {

  public EmailChangeValidator () {
    RuleFor (x => x.Request.OldEmail)
     .NotEmpty ().WithMessage ("Старая почта обязательна!")
     .EmailAddress ().WithMessage ("Неверный формат почты!");
    
    RuleFor (x => x.Request.NewEmail)
     .NotEmpty ().WithMessage ("Новая почта обязательна!")
     .EmailAddress ().WithMessage ("Неверный формат почты!");

    RuleFor (x => x.Request.ConfirmNewEmail)
     .NotEmpty ().WithMessage ("Повтор почты обязателен!")
     .Equal (x => x.Request.NewEmail).WithMessage ("Почты не совпадают");
  }

}