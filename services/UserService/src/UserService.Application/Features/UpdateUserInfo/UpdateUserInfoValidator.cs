using FluentValidation;

namespace UserService.Application.Features.UpdateUserInfo;

public class UpdateUserInfoValidator : AbstractValidator<UpdateUserInfoCommand> {

    public UpdateUserInfoValidator() {
        RuleFor(u => u.UserInfoUpdateDTO.UserName)
            .NotEmpty().WithMessage("Имя пользователя не должно быть пустым");

        RuleFor(u => u.UserInfoUpdateDTO.GitHubUrl)
            .Must(BeAValidUrl).WithMessage("Не валидный Url")
            .When(u => !string.IsNullOrWhiteSpace(u.UserInfoUpdateDTO.GitHubUrl));

        RuleFor(u => u.UserInfoUpdateDTO.WebsiteUrl)
            .Must(BeAValidUrl).WithMessage("Не валидный Url")
            .When(u => !string.IsNullOrWhiteSpace(u.UserInfoUpdateDTO.WebsiteUrl));
    }

    private static bool BeAValidUrl( string? url ) {      
        return Uri.TryCreate(url, UriKind.Absolute, out var uri)
               && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}