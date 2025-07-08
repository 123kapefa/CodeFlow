using Ardalis.Result;
using Contracts.Commands;
using UserService.Domain.Entities;
using UserService.Domain.Repositories;

namespace UserService.Application.Features.GetUserFullInfo;

public class GetUserFullInfoHandler : ICommandHandler<Result<UserInfo>, GetUserFullInfoCommand> {

    private readonly IUserInfoRepository _userInfoRepository;

    public GetUserFullInfoHandler( IUserInfoRepository userInfoRepository ) {
        _userInfoRepository = userInfoRepository;
    }

    public async Task<Result<Result<UserInfo>>> Handle( GetUserFullInfoCommand command, CancellationToken cancellationToken ) {
        return await _userInfoRepository.GetFullUserInfoByIdAsync(command.UserId, cancellationToken);
    }
    
}