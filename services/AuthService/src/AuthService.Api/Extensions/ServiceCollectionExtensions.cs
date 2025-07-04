using AuthService.Domain.Repositories;
using AuthService.Infrastructure.Repositories;

namespace AuthService.Api.Extensions;

public static class ServiceCollectionExtensions {

  public static WebApplicationBuilder UseCustomServices(this WebApplicationBuilder builder)
  {
    builder.Services.AddScoped<IUserDataRepository, UserDataRepository>();

    return builder;
  }

}