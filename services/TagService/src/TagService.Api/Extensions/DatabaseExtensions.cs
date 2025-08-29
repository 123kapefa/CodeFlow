using TagService.Infrastructure.Data;

namespace TagService.Api.Extensions;

public static class DatabaseExtensions {

  public static WebApplicationBuilder AddDatabase (this WebApplicationBuilder builder) {
    builder.Services.AddScoped<TagServiceDbContext> (_ =>
      new TagServiceDbContext (builder.Configuration.GetConnectionString (nameof(TagServiceDbContext))!));

    return builder;
  }
  
  // public static IApplicationBuilder UseDatabase (this IApplicationBuilder app) {
  //   
  //   using (var scope = app.Services.CreateScope())
  //   {
  //     var services = scope.ServiceProvider;
  //     try
  //     {
  //       var context = services.GetRequiredService<TagServiceDbContext>();
  //       context.Database.Migrate();
  //     }
  //     catch (Exception ex)
  //     {
  //       Console.WriteLine($"Ошибка при выполнении миграций: {ex.Message}");
  //       throw;
  //     }
  //   }
  //   
  //   return app;
  // }

}