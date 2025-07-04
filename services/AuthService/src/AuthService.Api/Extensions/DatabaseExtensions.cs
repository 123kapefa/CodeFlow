using AuthService.Infrastructure;

namespace AuthService.Api.Extensions;

public static class DatabaseExtensions {

  public static WebApplicationBuilder UseDatabase (this WebApplicationBuilder builder) {
    builder.Services.AddScoped<AuthServiceDbContext> (_ =>
      new AuthServiceDbContext (builder.Configuration.GetConnectionString (nameof(AuthServiceDbContext))!));

    return builder;
  }

}