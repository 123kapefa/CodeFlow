using Microsoft.AspNetCore.Identity;

namespace AuthService.Domain.Entities;

public class UserData : IdentityUser<Guid> {

  public bool IsExternal { get; set; }

}