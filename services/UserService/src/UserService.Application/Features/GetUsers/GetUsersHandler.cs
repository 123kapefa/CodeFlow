using Abstractions.Commands;

using Ardalis.Result;

using Contracts.DTOs.UserService;

using UserService.Domain.Repositories;

namespace UserService.Application.Features.GetUsers;

public class GetUsersHandler : ICommandHandler<PagedResult<IEnumerable<UserShortDTO>>, GetUsersCommand> {

    private readonly IUserInfoRepository _userInfoRepository;

    public GetUsersHandler( IUserInfoRepository userInfoRepository ) {
        _userInfoRepository = userInfoRepository;
    }

    public async Task<Result<PagedResult<IEnumerable<UserShortDTO>>>> Handle( GetUsersCommand command, CancellationToken cancellationToken ) {
        var usersResult = await _userInfoRepository.GetUsersAsync(command.PageParams, command.SortParams, cancellationToken);

        if(!usersResult.IsSuccess) {
            return Result<PagedResult<IEnumerable<UserShortDTO>>>.Error(usersResult.Errors.First().ToString());
        }

        IEnumerable<UserShortDTO> usersInfoDTO = usersResult.Value.items.Select(i => new UserShortDTO {
            UserId = i.UserId,
            UserName = i.Username,
            Location = i.Location,
            AboutMe = i.AboutMe,
            AvatarUrl = i.AvatarUrl,
            Reputation = i.UserStatistic.Reputation,
            Tags = [] //TODO полуние TagDTO из другого сервиса
        });

        return Result<PagedResult<IEnumerable<UserShortDTO>>>
            .Success(new PagedResult<IEnumerable<UserShortDTO>>(usersResult.Value.pageInfo, usersInfoDTO));
    }
}