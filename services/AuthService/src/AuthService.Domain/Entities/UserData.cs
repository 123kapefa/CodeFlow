using Microsoft.AspNetCore.Identity;

namespace AuthService.Domain.Entities;

public class UserData : IdentityUser<Guid> {

  public bool IsExternal { get; set; }
  public string? Fullname { get; set; }

  public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken> ();

  protected UserData () { }

  private UserData (string fullname) {
    Fullname = fullname;
  }

  public static UserData Create (string email) {
    return new UserData () { Email = email, UserName = email };
  }

  public static UserData Create (string fullname, string email) {
    return new UserData (fullname) { Email = email, UserName = email };
  }

}