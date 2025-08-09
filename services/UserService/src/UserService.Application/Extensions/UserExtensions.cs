using Contracts.DTOs.UserService;

using UserService.Domain.Entities;

namespace UserService.Application.Extensions;

public static class UserExtensions {

  public static UserShortDTO ToUserShortDto (this UserInfo user) =>
    new UserShortDTO {
      UserName = user.Username
      , Location = user.Location
      , AboutMe = user.AboutMe
      , AvatarUrl = user.AvatarUrl
      , Reputation = user.UserStatistic.Reputation
      , Tags = []
    };

  public static IEnumerable<UserForQuestionDto> ToUsersForQuestionDto (this IEnumerable<UserInfo> users)
    => users.Select(user => new UserForQuestionDto (
      user.UserId,
      user.Username,
      user.UserStatistic.Reputation,
      user.AvatarUrl
    ));


  public static IEnumerable<UserShortDTO> ToUsersShortDto (this IEnumerable<UserInfo> users)
    => users.Select(user => new UserShortDTO {
      UserName = user.Username,
      Location = user.Location,
      AboutMe = user.AboutMe,
      AvatarUrl = user.AvatarUrl,
      Reputation = user.UserStatistic.Reputation,
      Tags = []
    });

}