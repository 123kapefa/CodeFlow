using VoteService.Infrastructure;

namespace VoteService.Api.Extensions;

public static class DatabaseExtensions {

  public static WebApplicationBuilder AddDatabase (this WebApplicationBuilder builder) {
    builder.Services.AddScoped<VoteServiceDbContext> (_ =>
      new VoteServiceDbContext (builder.Configuration.GetConnectionString (nameof(VoteServiceDbContext))!));

    return builder;
  }

}