using System.Security.Claims;

namespace Auth.Extensions;

public static class ClaimsPrincipalExtensions
{

  public static Guid GetUserId (this ClaimsPrincipal user) {
    throw new NotImplementedException ();
  }

  public static string? GetEmail (this ClaimsPrincipal user) {
    throw new NotImplementedException ();
  }

  public static bool HasPermission (this ClaimsPrincipal user, string permission) {
    throw new NotImplementedException (); 
  }
}
