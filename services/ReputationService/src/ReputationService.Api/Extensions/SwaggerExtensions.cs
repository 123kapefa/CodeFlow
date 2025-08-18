using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace ReputationService.Api.Extensions;

public static class SwaggerExtensions {

  public static WebApplicationBuilder AddCustomSwagger (this WebApplicationBuilder builder) {
    
    builder.Services.AddOpenApi ();
    builder.Services.AddSwaggerGen (options => {
      options.SwaggerDoc ("v1", new OpenApiInfo {
        Title = "AnswerService API",
        Version = "v1",
        Description = "Swagger AnswerService"
      });

      options.EnableAnnotations();
    });
    
    return builder;
  }

  public static IApplicationBuilder UseCustomSwagger (this IApplicationBuilder app) {
    
    app.UseSwagger ();
    app.UseSwaggerUI (options => {
      options.SwaggerEndpoint ("/swagger/v1/swagger.json", "AnswerService API v1");
    });

    return app;
  }
}