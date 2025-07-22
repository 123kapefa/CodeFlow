using Microsoft.OpenApi.Models;

namespace UserService.Api.Extensions;

public static class SwaggerExtensions {

  public static WebApplicationBuilder UseCustomSwagger (this WebApplicationBuilder builder) {
    
    builder.Services.AddSwaggerGen (options => {
      options.SwaggerDoc ("v1", new OpenApiInfo {
        Title = "UserService API",
        Version = "v1",
        Description = "Swagger UserService"
      });

      options.EnableAnnotations();
    });
    
    return builder;
  }

  public static IApplicationBuilder UseCustomSwagger (this IApplicationBuilder app) {
    
    app.UseSwagger ();
    app.UseSwaggerUI (options => {
      options.SwaggerEndpoint ("/swagger/v1/swagger.json", "UserService API v1");
    });

    return app;
  }
}