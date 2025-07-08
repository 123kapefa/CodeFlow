using Ardalis.Result;
using Contracts.Commands;
using UserService.Domain.Entities;
using UserService.Domain.Repositories;

namespace UserService.Application.Features.UpdateUserVisit;

public class UpdateUserVisitHandler : ICommandHandler<UpdateUserVisitCommand> {

    private readonly IUserInfoRepository _userInfoRepository;

    public UpdateUserVisitHandler( IUserInfoRepository userInfoRepository ) {
        _userInfoRepository = userInfoRepository;
    }

    public async Task<Result> Handle( UpdateUserVisitCommand command, CancellationToken cancellationToken ) {
        Result<UserStatistic> user = await _userInfoRepository.GetUserStatisticByIdAsync(command.UserId, cancellationToken);

        if(!user.IsSuccess)
            return Result.Error(new ErrorList(user.Errors));
        

        if(user.Value.LastVisitAt.Date != DateTime.UtcNow.Date) {
            user.Value.LastVisitAt = DateTime.UtcNow;
            user.Value.VisitCount += 1;
        }

        Result result = await _userInfoRepository.UpdateUserStatisticAsync(user.Value, cancellationToken);

        return result.IsSuccess ? Result.Success() : Result.Error(new ErrorList(result.Errors));
    }

}