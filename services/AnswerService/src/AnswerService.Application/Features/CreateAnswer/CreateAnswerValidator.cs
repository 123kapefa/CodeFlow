using AnswerService.Application.Requests;

using FluentValidation;

namespace AnswerService.Application.Features.CreateAnswer;

public class CreateAnswerValidator : AbstractValidator<CreateAnswerRequest> {

  public CreateAnswerValidator () {
    RuleFor (x => x.Content)
     .NotEmpty ().WithMessage ("Контент не может быть пустым.");
  }

}