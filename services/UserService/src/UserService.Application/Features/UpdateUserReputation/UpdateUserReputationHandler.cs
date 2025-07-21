using Abstractions.Commands;

using Ardalis.Result;

using UserService.Domain.Entities;
using UserService.Domain.Repositories;

namespace UserService.Application.Features.UpdateUserReputation;

public class UpdateUserReputationHandler : ICommandHandler<UpdateUserReputationCommand> {

    private readonly IUserInfoRepository _userInfoRepository;

    public UpdateUserReputationHandler( IUserInfoRepository userInfoRepository ) {
        _userInfoRepository = userInfoRepository;
    }

    public async Task<Result> Handle( UpdateUserReputationCommand command, CancellationToken cancellationToken ) {
        Result<UserStatistic> user = await _userInfoRepository
                    .GetUserStatisticByIdAsync(command.UserId, cancellationToken);

        if(!user.IsSuccess)
            return Result.Error(new ErrorList(user.Errors));

        user.Value.Reputation = command.Reputation;

        Result updateResult = await _userInfoRepository
                                .UpdateUserStatisticAsync(user.Value, cancellationToken);

        return updateResult.IsSuccess ? Result.Success() : Result.Error(new ErrorList(updateResult.Errors));
    }

}