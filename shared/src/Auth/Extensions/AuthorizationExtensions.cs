using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Extensions;

public static class AuthorizationExtensions {

  public static IServiceCollection AddJwtAuth (this IServiceCollection services, IConfiguration config) {
    throw new NotImplementedException ();
  }

}