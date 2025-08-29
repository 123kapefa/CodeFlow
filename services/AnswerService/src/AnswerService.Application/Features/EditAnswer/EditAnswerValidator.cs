using FluentValidation;

namespace AnswerService.Application.Features.EditAnswer;

public class EditAnswerValidator : AbstractValidator<EditAnswerCommand> {

  public EditAnswerValidator () {
    RuleFor (x => x.Request.Content)
     .NotEmpty ().WithMessage ("Контент не может быть пустым.");
  }

}