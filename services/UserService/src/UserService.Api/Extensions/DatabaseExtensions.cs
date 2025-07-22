using UserService.Infrastructure.Data;

namespace UserService.Api.Extensions;

public static class DatabaseExtensions {

  public static WebApplicationBuilder AddDatabase (this WebApplicationBuilder builder) {
    builder.Services.AddScoped<UserServiceDbContext> (_ =>
      new UserServiceDbContext (builder.Configuration.GetConnectionString (nameof(UserServiceDbContext))!));

    return builder;
  }

}