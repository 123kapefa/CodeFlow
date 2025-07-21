using FluentValidation;

namespace AnswerService.Application.Features.CreateAnswer;

public class CreateAnswerValidator : AbstractValidator<CreateAnswerCommand> {

  public CreateAnswerValidator () {
    RuleFor (x => x.Request.Content)
     .NotEmpty ().WithMessage ("Контент не может быть пустым.");
  }

}