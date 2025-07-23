using FluentValidation;
using QuestionService.Application.Features.CreateQuestion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionService.Application.Features.UpdateQuestion;

public class UpdateQuestionValidator : AbstractValidator<UpdateQuestionCommand> {

    public UpdateQuestionValidator() {
        RuleFor(q => q.UpdateQuestionDTO.UserEditorId)
            .NotEmpty().WithMessage("Id автора вопроса не может быть пустым");        

        RuleFor(q => q.UpdateQuestionDTO.Content)
            .NotEmpty().WithMessage("Тело запроса не может быть пустым");

        RuleFor(q => q.UpdateQuestionDTO.QuestionTagsDTO)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Список тэгов обязателен")
            .NotEmpty().WithMessage("Нужно указать минимум один тег");
    }

}
