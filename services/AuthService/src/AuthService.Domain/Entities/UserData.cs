using Microsoft.AspNetCore.Identity;

namespace AuthService.Domain.Entities;

public class UserData : IdentityUser<Guid> {

  public bool IsExternal { get; set; }

  protected UserData() {}

  private UserData (string email) {}
  
  public static UserData Create (string email) {
    return new UserData(email);
  }
}