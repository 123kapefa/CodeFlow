using Ardalis.Result;
using Contracts.Commands;
using UserService.Domain.Repositories;

namespace UserService.Application.Features.DeleteUser;

public class DeleteUserHandler : ICommandHandler<DeleteUserCommand> {

    private readonly IUserInfoRepository _userInfoRepository;

    public DeleteUserHandler( IUserInfoRepository userInfoRepository ) {
        _userInfoRepository = userInfoRepository;
    }

    public async Task<Result> Handle( DeleteUserCommand command, CancellationToken cancellationToken ) {
        Result result = await _userInfoRepository.DeleteUserInfoAsync(command.UserId, cancellationToken);

        return result.IsSuccess ? Result.Success() : Result.Error(new ErrorList(result.Errors));
    }

}