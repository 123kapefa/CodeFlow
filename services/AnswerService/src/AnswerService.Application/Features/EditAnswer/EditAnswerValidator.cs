using AnswerService.Application.Requests;

using FluentValidation;

namespace AnswerService.Application.Features.EditAnswer;

public class EditAnswerValidator : AbstractValidator<UpdateAnswerRequest> {

  public EditAnswerValidator () {
    RuleFor (x => x.Content)
     .NotEmpty ().WithMessage ("Контент не может быть пустым.");
  }

}