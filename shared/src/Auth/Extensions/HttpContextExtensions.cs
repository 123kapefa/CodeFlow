using Microsoft.AspNetCore.Http;

namespace Auth.Extensions;

public static class HttpContextExtensions {

  public static Guid GetUserId (this HttpContext context) => context.User.GetUserId ();

}