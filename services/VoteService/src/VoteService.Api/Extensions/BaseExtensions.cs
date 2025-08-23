using Auth.Extensions;

namespace VoteService.Api.Extensions;

public static class BaseExtensions {

  public static WebApplicationBuilder AddBase (this WebApplicationBuilder builder) {
    builder.Services.AddControllers ();
    builder.Services.AddEndpointsApiExplorer ();
    builder.Services.AddOpenApi ();

    if (builder.Environment.IsDevelopment()) {
      DotNetEnv.Env.TraversePath().Load();
    }
    
    return builder;
  }

  public static IApplicationBuilder UseBase (this IApplicationBuilder app) {
    
    app.UseDeveloperExceptionPage ();
    
    return app;
  }
  
}