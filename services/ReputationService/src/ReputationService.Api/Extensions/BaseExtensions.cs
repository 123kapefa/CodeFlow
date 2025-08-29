using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ReputationService.Api.Extensions;

public static class BaseExtensions {

  public static WebApplicationBuilder AddBase (this WebApplicationBuilder builder) {
    builder.Services.AddControllers ();
    builder.Services.AddEndpointsApiExplorer ();
    builder.Services.AddOpenApi ();

    return builder;
  }

  public static IApplicationBuilder UseBase (this IApplicationBuilder app) {
    
    app.UseDeveloperExceptionPage ();
    
    return app;
  }
  
}