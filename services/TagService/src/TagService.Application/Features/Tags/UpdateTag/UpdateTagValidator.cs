using FluentValidation;

namespace TagService.Application.Features.Tags.UpdateTag;

public class UpdateTagValidator : AbstractValidator<UpdateTagCommand> {

    public UpdateTagValidator() {
        RuleFor(x => x.TagUpdateDTO.Name)         
         .MaximumLength(64)
         .WithMessage("Имя не длиннее 64 символов")
         .Matches(@"^[a-z0-9\.\+\-]+$")  
         .WithMessage("Допустимы строчные латинские буквы, цифры, +, -, .")
         .When(x => x.TagUpdateDTO.Description is not null);

        RuleFor(x => x.TagUpdateDTO.Description)
            .MaximumLength(256)
            .WithMessage("Описание не длиннее 256 символов")
            .When(x => x.TagUpdateDTO.Description is not null);
    }

}
