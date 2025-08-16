using Auth.Extensions;

using StackExchange.Redis;

namespace QuestionService.Api.Extensions;

public static class BaseExtensions {

  public static WebApplicationBuilder AddBase (this WebApplicationBuilder builder) {
    builder.Services.AddControllers ();
    builder.Services.AddEndpointsApiExplorer ();
    builder.Services.AddOpenApi ();

    if (builder.Environment.IsDevelopment()) {
      DotNetEnv.Env.TraversePath().Load();
    }
    
    builder.Services.AddJwtAuth(builder.Configuration);
    
    builder.Services.AddSingleton<IConnectionMultiplexer>(_ => {
      var cs = builder.Configuration.GetConnectionString("Redis")
     ?? throw new InvalidOperationException("Redis connection string is missing");
      return ConnectionMultiplexer.Connect(cs);
    });
    
    return builder;
  }

  public static IApplicationBuilder UseBase (this IApplicationBuilder app) {
    
    app.UseDeveloperExceptionPage ();
    
    return app;
  }
  
}