using CommentService.Infrastructure.Data;

namespace CommentService.Api.Extensions;

public static class DatabaseExtensions {

  public static WebApplicationBuilder AddDatabase (this WebApplicationBuilder builder) {
    builder.Services.AddScoped<CommentServiceDbContext> (_ =>
      new CommentServiceDbContext (builder.Configuration.GetConnectionString (nameof(CommentServiceDbContext))!));

    return builder;
  }

}