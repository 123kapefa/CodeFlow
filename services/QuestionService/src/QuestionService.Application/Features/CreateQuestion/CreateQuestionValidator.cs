using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionService.Application.Features.CreateQuestion;

public class CreateQuestionValidator : AbstractValidator<CreateQuestionCommand> {

    public CreateQuestionValidator() {
        RuleFor(q => q.QuestionDto.UserId)
            .NotEmpty().WithMessage("Id автора вопроса не может быть пустым");

        RuleFor(q => q.QuestionDto.Title)
            .NotEmpty().WithMessage("Заголовок вопроса не может быть пустым");

        RuleFor(q => q.QuestionDto.Content)
            .NotEmpty().WithMessage("Тело запроса не может быть пустым");

        RuleFor(q => q.QuestionDto.NewTags)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Список тэгов обязателен")
            .NotEmpty().WithMessage("Нужно указать минимум один тег");
    }

}
