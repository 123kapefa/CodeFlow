using Abstractions.Commands;

using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using UserService.Domain.Entities;
using UserService.Domain.Repositories;

namespace UserService.Application.Features.UpdateUserInfo;

public class UpdateUserInfoHandler : ICommandHandler<UpdateUserInfoCommand> {

    private readonly IUserInfoRepository _userInfoRepository;
    private readonly IValidator<UpdateUserInfoCommand> _validator;

    public UpdateUserInfoHandler( IUserInfoRepository userInfoRepository, IValidator<UpdateUserInfoCommand> validator ) {
        _userInfoRepository = userInfoRepository;
        _validator = validator;
    }

    public async Task<Result> Handle( UpdateUserInfoCommand command, CancellationToken cancellationToken ) {
        var validateResult = await _validator.ValidateAsync(command);

        if(!validateResult.IsValid)
            return Result.Invalid(validateResult.AsErrors());

        Result<UserInfo> userInfo = await _userInfoRepository.GetUserInfoByIdAsync(command.UserInfoUpdateDTO.UserId, cancellationToken);

        if(!userInfo.IsSuccess)
            return Result.Error(new ErrorList(userInfo.Errors));

        userInfo.Value.Username = command.UserInfoUpdateDTO.UserName;
        userInfo.Value.Location = command.UserInfoUpdateDTO.Location;
        userInfo.Value.AvatarUrl = command.UserInfoUpdateDTO.AvatarUrl;
        userInfo.Value.GitHubUrl = command.UserInfoUpdateDTO.GitHubUrl;
        userInfo.Value.WebsiteUrl = command.UserInfoUpdateDTO.WebsiteUrl;
        userInfo.Value.AboutMe = command.UserInfoUpdateDTO.AboutMe;

        Result result = await _userInfoRepository.UpdateUserInfoAsync(userInfo.Value, cancellationToken);

        return result.IsSuccess ? Result.Success() : Result.Error(new ErrorList(result.Errors));
    }

}
