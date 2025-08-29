using QuestionService.Infrastructure.Data;

namespace QuestionService.Api.Extensions;

public static class DatabaseExtensions {

  public static WebApplicationBuilder AddDatabase (this WebApplicationBuilder builder) {
    builder.Services.AddScoped<QuestionServiceDbContext> (_ =>
      new QuestionServiceDbContext (builder.Configuration.GetConnectionString (nameof(QuestionServiceDbContext))!));

    return builder;
  }

}