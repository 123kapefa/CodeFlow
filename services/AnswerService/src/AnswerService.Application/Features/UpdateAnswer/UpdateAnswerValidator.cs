using AnswerService.Application.Requests;

using FluentValidation;

namespace AnswerService.Application.Features.UpdateAnswer;

public class UpdateAnswerValidator : AbstractValidator<CreateAnswerRequest> {

  public UpdateAnswerValidator () {
    RuleFor (x => x.Content)
     .NotEmpty ().WithMessage ("Контент не может быть пустым.");
  }

}