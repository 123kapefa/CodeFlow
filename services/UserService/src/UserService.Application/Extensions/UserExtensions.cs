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