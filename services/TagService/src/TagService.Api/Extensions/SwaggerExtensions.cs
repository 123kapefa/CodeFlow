using Microsoft.OpenApi.Models;

namespace TagService.Api.Extensions;

public static class SwaggerExtensions {

  public static WebApplicationBuilder UseCustomSwagger (this WebApplicationBuilder builder) {
    
    builder.Services.AddSwaggerGen (options => {
      options.SwaggerDoc ("v1", new OpenApiInfo {
        Title = "TagService API",
        Version = "v1",
        Description = "Swagger TagService"
      });

      options.EnableAnnotations();
    });
    
    return builder;
  }

  public static IApplicationBuilder UseCustomSwagger (this IApplicationBuilder app) {
    
    app.UseSwagger ();
    app.UseSwaggerUI (options => {
      options.SwaggerEndpoint ("/swagger/v1/swagger.json", "TagService API v1");
    });

    return app;
  }
}