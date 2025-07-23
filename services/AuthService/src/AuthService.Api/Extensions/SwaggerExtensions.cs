using Microsoft.OpenApi.Models;

namespace AuthService.Api.Extensions;

public static class SwaggerExtensions {

  public static WebApplicationBuilder AddCustomSwagger (this WebApplicationBuilder builder) {
    
    builder.Services.AddSwaggerGen (options => {
      options.SwaggerDoc ("v1", new OpenApiInfo {
        Title = "AuthService API",
        Version = "v1",
        Description = "Swagger AuthService"
      });

      options.EnableAnnotations();
    });
    
    return builder;
  }

  public static IApplicationBuilder UseCustomSwagger (this IApplicationBuilder app) {
    
    app.UseSwagger ();
    app.UseSwaggerUI (options => {
      options.SwaggerEndpoint ("/swagger/v1/swagger.json", "AuthService API v1");
    });

    return app;
  }
}