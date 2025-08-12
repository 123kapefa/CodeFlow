using Abstractions.Commands;

using Ardalis.Result;

using Contracts.DTOs.UserService;

using UserService.Application.Extensions;
using UserService.Domain.Repositories;

namespace UserService.Application.Features.GetUsersByIds;

public class GetUsersByIdsHandler : ICommandHandler<IEnumerable<UserForQuestionDto>, GetUsersByIdsCommand> {

  private readonly IUserInfoRepository _userInfoRepository;
  
  public GetUsersByIdsHandler (IUserInfoRepository userInfoRepository) {
    _userInfoRepository = userInfoRepository;
  }

  public async Task<Result<IEnumerable<UserForQuestionDto>>> Handle (GetUsersByIdsCommand command, CancellationToken cancellationToken) {
    var result = await _userInfoRepository.GetUsersByIdsAsync(command.UserIds, cancellationToken);

    if (!result.IsSuccess) {
      return Result<IEnumerable<UserForQuestionDto>>.Error (result.Errors.First ());
    }

    return Result<IEnumerable<UserForQuestionDto>>.Success (result.Value.ToUsersForQuestionDto());
  }

}