using Abstractions.Commands;

using Ardalis.Result;

using Contracts.DTOs.UserService;

using UserService.Application.Extensions;
using UserService.Domain.Repositories;

namespace UserService.Application.Features.GetUsers;

public class GetUsersHandler : ICommandHandler<PagedResult<IEnumerable<UserShortDTO>>, GetUsersCommand> {

    private readonly IUserInfoRepository _userInfoRepository;

    public GetUsersHandler( IUserInfoRepository userInfoRepository ) {
        _userInfoRepository = userInfoRepository;
    }

    public async Task<Result<PagedResult<IEnumerable<UserShortDTO>>>> Handle( GetUsersCommand command, CancellationToken cancellationToken ) {
        var usersResult = await _userInfoRepository.GetUsersAsync(command.PageParams, command.SortParams, command.SearchFilter, cancellationToken);

        if(!usersResult.IsSuccess) {
            return Result<PagedResult<IEnumerable<UserShortDTO>>>.Error(usersResult.Errors.First().ToString());
        }

        IEnumerable<UserShortDTO> usersInfoDto = usersResult.Value.items.ToUsersShortDto ();

        return Result<PagedResult<IEnumerable<UserShortDTO>>>
            .Success(new PagedResult<IEnumerable<UserShortDTO>>(usersResult.Value.pageInfo, usersInfoDto));
    }
}