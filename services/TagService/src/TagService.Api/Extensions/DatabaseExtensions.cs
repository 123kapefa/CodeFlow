using TagService.Infrastructure.Data;

namespace TagService.Api.Extensions;

public static class DatabaseExtensions {

  public static WebApplicationBuilder AddDatabase (this WebApplicationBuilder builder) {
    builder.Services.AddScoped<TagServiceDbContext> (_ =>
      new TagServiceDbContext (builder.Configuration.GetConnectionString (nameof(TagServiceDbContext))!));

    return builder;
  }

}