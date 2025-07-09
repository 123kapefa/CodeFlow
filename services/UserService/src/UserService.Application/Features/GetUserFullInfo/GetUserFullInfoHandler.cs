using Ardalis.Result;
using Contracts.Commands;
using UserService.Application.DTO;
using UserService.Domain.Entities;
using UserService.Domain.Repositories;

namespace UserService.Application.Features.GetUserFullInfo;

public class GetUserFullInfoHandler : ICommandHandler<Result<UserFullInfoDTO>, GetUserFullInfoCommand> {

    private readonly IUserInfoRepository _userInfoRepository;

    public GetUserFullInfoHandler( IUserInfoRepository userInfoRepository ) {
        _userInfoRepository = userInfoRepository;
    }

    public async Task<Result<Result<UserFullInfoDTO>>> Handle( GetUserFullInfoCommand command, CancellationToken cancellationToken ) {
        Result<UserInfo> result =  await _userInfoRepository.GetFullUserInfoByIdAsync(command.UserId, cancellationToken);

        if(!result.IsSuccess)
            return Result<UserFullInfoDTO>.Error(new ErrorList(result.Errors));

        UserFullInfoDTO userFullUnfo = new UserFullInfoDTO {             
            CreatedAt = result.Value.CreatedAt,
            UserName = result.Value.Username,
            AvatarUrl = result.Value.AvatarUrl,
            AboutMe = result.Value.AboutMe,
            Location = result.Value.Location,
            WebsiteUrl = result.Value.WebsiteUrl,
            GitHubUrl = result.Value.GitHubUrl,
            LastVisitAt = result.Value.UserStatistic.LastVisitAt,
            VisitCount = result.Value.UserStatistic.VisitCount,
            Reputation = result.Value.UserStatistic.Reputation,
        };

        return Result.Success( userFullUnfo );
    }
    
}