using Microsoft.OpenApi.Models;

namespace AuthService.Api.Extensions;

public static class BaseExtensions {

  public static WebApplicationBuilder UseBase (this WebApplicationBuilder builder) {
    builder.Services.AddControllers ();
    builder.Services.AddEndpointsApiExplorer ();
    builder.Services.AddOpenApi ();
    
    builder.Services.AddSwaggerGen (options => {
      options.SwaggerDoc ("v1", new OpenApiInfo {
        Title = "Product API",
        Version = "v1",
        Description = "Swagger AuthService"
      });
    });

    return builder;
  }

}