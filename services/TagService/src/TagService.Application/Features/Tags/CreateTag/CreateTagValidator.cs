using FluentValidation;

namespace TagService.Application.Features.Tags.CreateTag;

public class CreateTagValidator : AbstractValidator<CreateTagCommand> {

    public CreateTagValidator() {
        RuleFor(x => x.TagCreateDto.Name)
            .NotEmpty().WithMessage("Имя тега обязательно")
            .MaximumLength(64).WithMessage("Имя не длиннее 64 символов")
            .Matches(@"^[a-z0-9\.\+\-]+$")  // как на Stack Overflow
            .WithMessage("Допустимы строчные латинские буквы, цифры, +, -, .");

        RuleFor(x => x.TagCreateDto.Description)
            .MaximumLength(256)
            .WithMessage("Описание не длиннее 256 символов")
            .When(x => x.TagCreateDto.Description is not null);
    }

}
