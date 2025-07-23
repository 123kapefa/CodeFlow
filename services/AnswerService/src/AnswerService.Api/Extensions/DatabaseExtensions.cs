using AnswerService.Infrastructure;

namespace AnswerService.Api.Extensions;

public static class DatabaseExtensions {

  public static WebApplicationBuilder AddDatabase (this WebApplicationBuilder builder) {
    builder.Services.AddScoped<AnswerServiceDbContext> (_ =>
      new AnswerServiceDbContext (builder.Configuration.GetConnectionString (nameof(AnswerServiceDbContext))!));

    return builder;
  }

}