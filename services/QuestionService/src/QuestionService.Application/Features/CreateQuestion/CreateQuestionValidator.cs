using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionService.Application.Features.CreateQuestion;

public class CreateQuestionCommand : AbstractValidator<CreateQuestionCommand> {

    public CreateQuestionCommand() {
        RuleFor(q => q.QuestionDTO.UserId)
            .NotEmpty().WithMessage("Id автора вопроса не может быть пустым");

        RuleFor(q => q.QuestionDTO.Title)
            .NotEmpty().WithMessage("Заголовок вопроса не может быть пустым");

        RuleFor(q => q.QuestionDTO.Content)
            .NotEmpty().WithMessage("Тело запроса не может быть пустым");

        RuleFor(q => q.QuestionDTO.QuestionTags)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Список тэгов обязателен")
            .Empty().WithMessage("Нужно указать минимум один тег");
    }

}