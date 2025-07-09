using Ardalis.Result;
using Contracts.Commands;
using UserService.Domain.Repositories;

namespace UserService.Application.Features.CreateUserInfo;

public class CreateUserInfoHandler : ICommandHandler<CreateUserInfoCommand> {

    private readonly IUserInfoRepository _userInfoRepository;

    public CreateUserInfoHandler( IUserInfoRepository userInfoRepository ) {
        _userInfoRepository = userInfoRepository;
    }

    public async Task<Result> Handle( CreateUserInfoCommand command, CancellationToken cancellationToken ) {
        Result result = 
            await _userInfoRepository.CreateUserInfoAsync(command.userId, command.userName, cancellationToken);

        return result.IsSuccess ? Result.Success() : Result.Error(new ErrorList(result.Errors));
    }

}
