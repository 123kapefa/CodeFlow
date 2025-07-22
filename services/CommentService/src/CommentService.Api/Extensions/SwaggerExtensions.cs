using Microsoft.OpenApi.Models;

namespace CommentService.Api.Extensions;

public static class SwaggerExtensions {

  public static WebApplicationBuilder UseCustomSwagger (this WebApplicationBuilder builder) {
    
    builder.Services.AddSwaggerGen (options => {
      options.SwaggerDoc ("v1", new OpenApiInfo {
        Title = "CommentService API",
        Version = "v1",
        Description = "Swagger CommentService"
      });

      options.EnableAnnotations();
    });
    
    return builder;
  }

  public static IApplicationBuilder UseCustomSwagger (this IApplicationBuilder app) {
    
    app.UseSwagger ();
    app.UseSwaggerUI (options => {
      options.SwaggerEndpoint ("/swagger/v1/swagger.json", "CommentService API v1");
    });

    return app;
  }
}